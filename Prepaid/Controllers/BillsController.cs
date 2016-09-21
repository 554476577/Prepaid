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
using System.Timers;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Prepaid.Controllers
{
    public class BillsController : ApiController
    {
        Timer timer;
        IBillRespository billRepository;
        IRoomRespository roomRespository;
        IDeviceRepository deviceRespository;

        public BillsController(IBillRespository billRepository, IRoomRespository roomRespository,IDeviceRepository deviceRespository)
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

        // GET: api/bills/recommendrooms
        [HttpGet]
        [Route("api/bills/recommendrooms")]
        public IHttpActionResult GetRecommendRooms()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<PrepaidBill> bills;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            string buildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetRecommendPrepaidBills(buildingNo);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetRecommendPrepaidBillsCount(buildingNo));
                bills = this.billRepository.GetRecommendPrepaidPagerBills(buildingNo, pageIndex, pageSize);
            }
            pager.Items = bills;

            return Ok(pager);
        }

        // GET: api/bills/arrearsrooms
        [HttpGet]
        [Route("api/bills/arrearsrooms")]
        public IHttpActionResult GetArrearsRooms()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<PrepaidBill> bills;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            string buildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetArrearsPrepaidBills(buildingNo);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetArrearsPrepaidBillsCount(buildingNo));
                bills = this.billRepository.GetArrearsPrepaidPagerBills(buildingNo, pageIndex, pageSize);
            }
            pager.Items = bills;

            return Ok(pager);
        }

        [HttpGet]
        [Route("api/export/prepaidbills")]
        public IHttpActionResult ExportPrepaidBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string flag = HttpContext.Current.Request.Params["Flag"];
            string buildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            string roomNo = HttpContext.Current.Request.Params["RoomNo"];
            string floor = HttpContext.Current.Request.Params["Floor"];
            string realName = HttpContext.Current.Request.Params["RealName"];
            IEnumerable<PrepaidBill> bills;
            if (flag == "0") // 获取所有欠费用户
                bills = this.billRepository.GetArrearsPrepaidBills(buildingNo);
            else if (flag == "1") // 获取所有优质用户
                bills = this.billRepository.GetRecommendPrepaidBills(buildingNo);
            else
                bills = this.billRepository.GetPrepaidBills(roomNo, buildingNo, floor, realName);

            string fileName = string.Format("业主能耗缴费实时账单({0}).xls",DateTime.Now.ToString("yyyy-MM-dd[hh-mm-ss]"));
            string[] titles = { "房间编号", "建筑编号", "业主姓名", "设备编号", "设备名称", "上次抄表读数", "当前抄表读数", "单价", 
                                  "价格","总能耗","总价","账户余额" };
            ReportHelper.ExportPrepaidBills(bills, titles, fileName);
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
                //bill.Remark = string.Format("yyyy-MM", bill.DateTime);
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
        public IHttpActionResult PostBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            PrepaidBill prepaidBill = this.billRepository.GetPrepaidBills(RoomNo, "", "", "").FirstOrDefault();
            DateTime now = DateTime.Now;
            string lotNo = TextHelper.GenerateUUID();
            using (TransactionScope ts = new TransactionScope())
            {
                foreach (PrepaidDeviceBill item in prepaidBill.PrepaidDeviceBills)
                {
                    Bill bill = new Bill();
                    bill.DeviceNo = item.DeviceNo;
                    bill.LotNo = lotNo;
                    bill.PreValue = item.PreValue ?? 0.00;
                    bill.CurValue = item.CurValue ?? 0.00;
                    bill.AccountBalance = prepaidBill.IntAccountBalance;
                    bill.Money = item.IntMoney;
                    bill.DateTime = now;
                    //bill.Remark = string.Format("yyyy-MM", bill.DateTime);
                    this.billRepository.Add(bill);

                    Device device = this.deviceRespository.GetByID(item.DeviceNo);
                    device.PreValue = item.CurValue;
                    this.deviceRespository.Put(device);
                }

                Room room = this.roomRespository.GetByID(RoomNo);
                room.AccountBalance = prepaidBill.IntBilledBalance;
                this.roomRespository.Put(room);

                ts.Complete(); // 提交事务
            }

            return Ok();
        }

        // POST: api/batch/bills
        [HttpPost]
        [Route("api/batch/bills")]
        [ResponseType(typeof(void))]
        public IHttpActionResult BatchPostBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            BatchSettle(false); // 批量结算

            return Ok();
        }

        // POST: api/timingbills/1
        [HttpPost]
        [Route("api/timingbills/{flag}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult TimingBills(int flag)
        {
            WebConfigHelper.WriteAppSetting("IsTimingSettle", flag.ToString());
            if (flag == 0 && this.timer != null) // 关闭定时器
            {
                this.timer.Stop();
            }
            else if (flag == 1)
            {
                DateTime now = DateTime.Now;
                DateTime lastDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1);
                lastDay = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 22, 0, 0); // 设定在本月最后一天结算
                TimeSpan span = lastDay - now;
                this.timer = new Timer();
                this.timer.Interval = span.Milliseconds;
                this.timer.Elapsed += timer_Elapsed;
                this.timer.Start();
            }

            return Ok();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime lastDay = now.AddMonths(1).AddDays(1);
            lastDay = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 22, 0, 0); // 设定在本月最后一天结算
            timer.Interval = (lastDay - now).Milliseconds;

            BatchSettle(true);
        }

        private void BatchSettle(bool isAuto)
        {
            IEnumerable<PrepaidBill> prepaidBills = this.billRepository.GetPrepaidBills();
            DateTime now = DateTime.Now;
            string lotNo = TextHelper.GenerateUUID();
            foreach (var prepaidBill in prepaidBills)
            {
                if (prepaidBill.IntBilledBalance < 0) // 如果余额不够，则不结算
                    continue;

                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (PrepaidDeviceBill item in prepaidBill.PrepaidDeviceBills)
                    {
                        Bill bill = new Bill();
                        bill.DeviceNo = item.DeviceNo;
                        bill.LotNo = lotNo;
                        bill.PreValue = item.PreValue ?? 0.00;
                        bill.CurValue = item.CurValue ?? 0.00;
                        bill.AccountBalance = prepaidBill.IntAccountBalance;
                        bill.Money = item.IntMoney;
                        bill.DateTime = now;
                        //bill.Remark = string.Format("yyyy-MM", bill.DateTime);
                        this.billRepository.Add(bill);

                        Device device = this.deviceRespository.GetByID(item.DeviceNo);
                        device.PreValue = item.CurValue;
                        this.deviceRespository.Put(device);
                    }

                    Room room = this.roomRespository.GetByID(prepaidBill.RoomNo);
                    if (!isAuto)
                        room.AccountBalance = prepaidBill.IntAccountBalance - prepaidBill.IntSumMoney;
                    else
                        room.AccountBalance = prepaidBill.IntAccountBalance - prepaidBill.IntSumMoney - prepaidBill.IntManagerFees;
                    this.roomRespository.Put(room);

                    ts.Complete(); // 提交事务
                }
            }
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