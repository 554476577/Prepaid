using Prepaid.Models;
using Prepaid.Repositories;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Prepaid.Controllers
{
    public class RechargesController : ApiController
    {
        IRechargeRespository rechargeRespository;
        IRoomRespository roomRespository;
        ILogRepository logRespository;

        public RechargesController(IRechargeRespository rechargeRespository, IRoomRespository userRespository, ILogRepository logRespository)
        {
            this.rechargeRespository = rechargeRespository;
            this.roomRespository = userRespository;
            this.logRespository = logRespository;
        }

        // GET: api/recharges
        public IHttpActionResult GetRecharges()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Recharge> recharges;
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                recharges = this.rechargeRespository.GetAll(RoomNo, RealName);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.rechargeRespository.GetCount(RoomNo, RealName));
                recharges = this.rechargeRespository.GetPagerItems(RoomNo, RealName, pageIndex, pageSize, u => u.DateTime, true);
            }

            var items = from item in recharges
                        select new
                        {
                            UUID = item.UUID,
                            RoomNo = item.RoomNo,
                            RealName = item.Room.RealName,
                            Money = TextHelper.ConvertMoney(item.Money),
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        [HttpGet]
        [Route("api/export/recharges")]
        public IHttpActionResult ExportRecharges()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string fileName = string.Format("业主充值记录{0}.xls", DateTime.Now.ToString("yyyy-MM-dd[hh-mm-ss]"));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UUID", "UUID");
            dict.Add("房间编号", "RoomNo");
            dict.Add("业主姓名", "RealName");
            dict.Add("充值金额", "Money");
            dict.Add("时间", "DateTime");
            dict.Add("备注", "Remark");

            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            IEnumerable<Recharge> recharges = this.rechargeRespository.GetAll(RoomNo, RealName);
            var items = from item in recharges
                        select new
                        {
                            UUID = item.UUID,
                            RoomNo = item.RoomNo,
                            RealName = item.Room.RealName,
                            Money = TextHelper.ConvertMoney(item.Money),
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            ReportHelper.Export(dict, items, fileName);
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.Write(fileName);

            return Ok();
        }

        // GET: api/recharges/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Recharge))]
        public async Task<IHttpActionResult> GetRecharge(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Recharge item = await this.rechargeRespository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                RoomNo = item.RoomNo,
                RealName = item.Room.RealName,
                Money = TextHelper.ConvertMoney(item.Money),
                DateTime = item.DateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/recharges/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRecharge(string uuid, [FromUri]Recharge recharge)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != recharge.UUID)
                return BadRequest();

            try
            {
                await this.rechargeRespository.PutAsync(recharge);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.rechargeRespository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/recharges
        [ResponseType(typeof(Recharge))]
        public IHttpActionResult PostRecharge([FromUri]Recharge recharge)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            // 检测两次充值时间间隔是否过于频繁，防止误充
            Setting setting = TextHelper.GetSystemConfig();
            if (setting.IsRechargeSettle)
            {
                bool isValid = ChkValidPeriod(recharge.RoomNo, setting.RechargeLimitInterval);
                if (!isValid)
                {
                    string msg = string.Format("两次充值时间间隔不到{0}分钟，请稍后重试!", setting.RechargeLimitInterval);
                    var errorResult = new Prepaid.Results.InternalServerErrorTextPlainResult(msg, Request);
                    errorResult.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    return errorResult;
                }
            }

            Room room = null;
            Log log = new Log();
            AdminSession admin = HttpContext.Current.Session["mySession"] as AdminSession;
            string money = TextHelper.ConvertMoney(recharge.Money);
            log.UserID = admin.UUID;
            log.Type = 2; // 1:登录日志 2:操作日志
            log.ClientAddr = TextHelper.GetHostAddress();
            log.Remark = "";
            log.DateTime = DateTime.Now;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    room = this.roomRespository.GetByID(recharge.RoomNo);
                    room.AccountBalance += recharge.Money;
                    this.roomRespository.Put(room);

                    recharge.UUID = TextHelper.GenerateUUID();
                    recharge.DateTime = DateTime.Now;
                    this.rechargeRespository.Add(recharge);

                    log.Content = string.Format("管理员:{0}对房间:{1}成功充值￥{2}元!", admin.UserName, recharge.RoomNo, money);
                    ts.Complete(); // 提交事务
                }
            }
            catch (DbUpdateException)
            {
                if (this.rechargeRespository.IsExist(recharge.UUID))
                    return Conflict();
                else
                    log.Content = string.Format("管理员:{0}对房间:{1}充值金额￥{2}元失败!", admin.UserName, recharge.RoomNo, money);
            }
            this.logRespository.Add(log);

            // 消息通知处理
            if (room != null)
                TextHelper.NotifyProcess(setting.Notify, room, money, log.Content);

            return Ok();
        }

        /// <summary>
        /// 检测连续两次充值的时间间隔。
        /// </summary>
        /// <param name="roomNo"></param>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        private bool ChkValidPeriod(string roomNo, int timeLimit)
        {
            var result = from item in this.rechargeRespository.GetAll(roomNo, "") orderby item.DateTime descending select item.DateTime;
            if (result.Count() == 0)
                return true;
            DateTime previous = result.First() ?? DateTime.MinValue;
            TimeSpan span = DateTime.Now - previous;
            if (span.TotalMinutes < timeLimit)
                return false;
            return true;
        }

        // DELETE: api/recharges/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteRecharge(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Recharge recharge = await this.rechargeRespository.GetByIdAsync(uuid);
            if (recharge == null)
                return NotFound();

            await this.rechargeRespository.DeleteAsync(recharge);

            return Ok();
        }
    }
}