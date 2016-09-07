using Newtonsoft.Json;
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
    public class BillsController : ApiController
    {
        IBillRespository billRepository;
        IRoomRespository roomRespository;
        IDeviceRepository deviceRespository;

        public BillsController(IBillRespository billRepository, IRoomRespository roomRespository, IDeviceRepository deviceRespository)
        {
            this.billRepository = billRepository;
            this.roomRespository = roomRespository;
            this.deviceRespository = deviceRespository;
        }

        // GET: api/bills
        public IHttpActionResult GetBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Bill> bills;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetCount());
                bills = this.billRepository.GetPagerItems(pageIndex, pageSize, u => u.LotNo);
            }

            var items = from item in bills
                        select new
                        {
                            ID = item.ID,
                            DeviceNo = item.DeviceNo,
                            DeviceName = item.Device.DeviceName,
                            RoomNo = item.Device.RoomNo,
                            PreValue = item.PreValue,
                            CurValue = item.CurValue,
                            Money = TextHelper.ConvertMoney(item.Money),
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/roombills
        [HttpGet]
        [Route("api/roombills")]
        public IHttpActionResult GetRoomBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<RoomBill> bills;
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string BuildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            string Floor = HttpContext.Current.Request.Params["Floor"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string StartTime = HttpContext.Current.Request.Params["StartTime"];
            string EndTime = HttpContext.Current.Request.Params["EndTime"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetRoomBills(RoomNo, BuildingNo, Floor, RealName, StartTime, EndTime);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetRoomBillsCount(RoomNo, BuildingNo, Floor, RealName, StartTime, EndTime));
                bills = this.billRepository.GetRoomPagerBills(RoomNo, BuildingNo, Floor, RealName, StartTime, EndTime, pageIndex, pageSize, u => u.RoomNo, true);
            }
            pager.Items = bills;

            return Ok(pager);
        }

        [HttpGet]
        [Route("api/export/roombills")]
        public IHttpActionResult ExportRoomBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string fileName = "业主能耗缴费历史账单.xls";
            string[] titles = { "房间编号", "结算批号", "业主姓名","结算时间","设备编号","设备名称", "上次刻度", "结算刻度", 
                                  "单价", "金额", "总能耗","总价格"};
            IEnumerable<RoomBill> userEnergies = this.billRepository.GetRoomBills();
            ReportHelper.ExportRoomBills(userEnergies, titles, fileName);
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.Write(fileName);

            return Ok();
        }

        // GET: api/prepaidbills
        [HttpGet]
        [Route("api/prepaidbills")]
        public IHttpActionResult GetPrepaidBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<PrepaidBill> bills;
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string BuildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            string Floor = HttpContext.Current.Request.Params["Floor"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetPrepaidBills(RoomNo, BuildingNo, Floor, RealName);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetPrepaidBillsCount(RoomNo, BuildingNo, Floor, RealName));
                bills = this.billRepository.GetPrepaidPagerBills(RoomNo, BuildingNo, Floor, RealName, pageIndex, pageSize, u => u.RoomNo);
            }
            pager.Items = bills;

            return Ok(pager);
        }

        // GET: api/recommendbills/1
        [HttpGet]
        [Route("api/recommendbills/{flag}")]
        public IHttpActionResult GetRecommendBills(int flag)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<PrepaidBill> bills;
            if (flag == 0)
                bills = this.billRepository.GetPrepaidPagerBills(1, 10, u => u.CreditScore);
            else
                bills = this.billRepository.GetPrepaidPagerBills(1, 10, u => u.CreditScore, true);

            return Ok(bills);
        }

        [HttpGet]
        [Route("api/export/prepaidbills")]
        public IHttpActionResult ExportPrepaidBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string fileName = "业主能耗缴费实时账单.xls";
            string[] titles = { "房间编号", "建筑编号", "业主姓名", "设备编号", "设备名称", "上次抄表读数", "当前抄表读数", "单价", 
                                  "价格","总能耗","总价","账户余额" };
            IEnumerable<PrepaidBill> prepaidEnergies = this.billRepository.GetPrepaidBills();
            ReportHelper.ExportPrepaidBills(prepaidEnergies, titles, fileName);
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.Write(fileName);

            return Ok();
        }

        // GET: api/bills/1
        [ResponseType(typeof(Bill))]
        public async Task<IHttpActionResult> GetBill(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Bill item = await this.billRepository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                DeviceNo = item.DeviceNo,
                DeviceName = item.Device.DeviceName,
                RoomNo = item.Device.RoomNo,
                PreValue = item.PreValue,
                CurValue = item.CurValue,
                Money = TextHelper.ConvertMoney(item.Money),
                DateTime = item.DateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/bills/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBill(int uuid, [FromUri]Bill bill)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != bill.ID)
                return BadRequest();

            try
            {
                bill.DateTime = DateTime.Now;
                await this.billRepository.PutAsync(bill);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.billRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/bills
        [ResponseType(typeof(Bill))]
        public async Task<IHttpActionResult> PostBill([FromUri]Bill bill)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                bill.LotNo = TextHelper.GenerateUUID();
                bill.DateTime = DateTime.Now;
                bill.Remark = string.Format("yyyy-MM", bill.DateTime);
                await this.billRepository.AddAsync(bill);
            }
            catch (DbUpdateException)
            {
                if (this.billRepository.IsExist(bill.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // POST: api/roombills
        [HttpPost]
        [Route("api/roombills")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string IntBilledBalance = HttpContext.Current.Request.Params["IntBilledBalance"];
            string strDeviceBills = HttpContext.Current.Request.Params["DeviceBills"];
            strDeviceBills = string.Format("[{0}]", strDeviceBills); // 格式化为json数组
            List<PrepaidDeviceBill> bills = JsonConvert.DeserializeObject<List<PrepaidDeviceBill>>(strDeviceBills);
            DateTime now = DateTime.Now;
            string lotNo = TextHelper.GenerateUUID();
            foreach (PrepaidDeviceBill item in bills)
            {
                Bill bill = new Bill();
                bill.DeviceNo = item.DeviceNo;
                bill.LotNo = lotNo;
                bill.PreValue = item.PreValue ?? 0.00;
                bill.CurValue = item.CurValue ?? 0.00;
                bill.Money = item.IntMoney;
                bill.DateTime = now;
                bill.Remark = string.Format("yyyy-MM", bill.DateTime);
                await this.billRepository.AddAsync(bill);

                Device device = await this.deviceRespository.GetByIdAsync(item.DeviceNo);
                device.PreValue = item.CurValue ?? 0.00;
                await this.deviceRespository.PutAsync(device);
            }

            Room room = await this.roomRespository.GetByIdAsync(RoomNo);
            room.AccountBalance = Convert.ToInt32(IntBilledBalance);
            await this.roomRespository.PutAsync(room);

            return Ok();
        }

        // POST: api/batch/bills
        [HttpPost]
        [Route("api/batch/bills")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> BatchPostBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<PrepaidBill> prepaidBills = this.billRepository.GetPrepaidBills();
            DateTime now = DateTime.Now;
            string lotNo = TextHelper.GenerateUUID();
            foreach (var prepaidBill in prepaidBills)
            {
                if (prepaidBill.IntAccountBalance < 0) // 如果预算不够，则不结算
                    continue;
                foreach (PrepaidDeviceBill item in prepaidBill.PrepaidDeviceBills)
                {
                    Bill bill = new Bill();
                    bill.DeviceNo = item.DeviceNo;
                    bill.LotNo = lotNo;
                    bill.PreValue = item.PreValue ?? 0.00;
                    bill.CurValue = item.CurValue ?? 0.00;
                    bill.Money = item.IntMoney;
                    bill.DateTime = now;
                    bill.Remark = string.Format("yyyy-MM", bill.DateTime);
                    await this.billRepository.AddAsync(bill);
                }

                Room room = await this.roomRespository.GetByIdAsync(prepaidBill.RoomNo);
                room.AccountBalance = prepaidBill.IntAccountBalance - prepaidBill.IntSumMoney;
                await this.roomRespository.PutAsync(room);
            }

            return Ok();
        }

        // DELETE: api/bills/1
        public async Task<IHttpActionResult> DeleteBill(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Bill bill = await this.billRepository.GetByIdAsync(uuid);
            if (bill == null)
                return NotFound();

            await this.billRepository.DeleteAsync(bill);

            return Ok();
        }
    }
}