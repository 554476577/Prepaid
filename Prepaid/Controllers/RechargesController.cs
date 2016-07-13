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
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Prepaid.Controllers
{
    public class RechargesController : ApiController
    {
        IRechargeRespository rechargeRepository;
        IUserRespository userRepository;

        public RechargesController(IRechargeRespository rechargeRepository, IUserRespository userRepository)
        {
            this.rechargeRepository = rechargeRepository;
            this.userRepository = userRepository;
        }

        // GET: api/recharges
        public IHttpActionResult GetRecharges()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Recharge> recharges;
            string UserID = HttpContext.Current.Request.Params["UserID"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                recharges = this.rechargeRepository.GetAll(UserID, RealName);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.rechargeRepository.GetCount(UserID, RealName));
                recharges = this.rechargeRepository.GetPagerItems(UserID, RealName, pageIndex, pageSize, u => u.DateTime,true);
            }

            var items = from item in recharges
                        select new
                        {
                            UUID = item.UUID,
                            UserID = item.UserID,
                            RealName = item.User.RealName,
                            Money = TextHelper.ConvertMoney(item.Money),
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/recharges/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Recharge))]
        public async Task<IHttpActionResult> GetRecharge(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Recharge item = await this.rechargeRepository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                UserID = item.UserID,
                RealName = item.User.RealName,
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
                await this.rechargeRepository.PutAsync(recharge);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.rechargeRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/recharges
        [ResponseType(typeof(Recharge))]
        public async Task<IHttpActionResult> PostRecharge([FromUri]Recharge recharge)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                string UserID = recharge.UserID;
                User user = this.userRepository.GetByID(UserID);
                user.AccountBalance += recharge.Money;
                await this.userRepository.PutAsync(user);

                recharge.UUID = TextHelper.GenerateUUID();
                recharge.DateTime = DateTime.Now;
                await this.rechargeRepository.AddAsync(recharge);
            }
            catch (DbUpdateException)
            {
                if (this.rechargeRepository.IsExist(recharge.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/recharges/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteRecharge(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Recharge recharge = await this.rechargeRepository.GetByIdAsync(uuid);
            if (recharge == null)
                return NotFound();

            await this.rechargeRepository.DeleteAsync(recharge);

            return Ok();
        }
    }
}