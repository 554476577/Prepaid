﻿using Newtonsoft.Json;
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

        public BillsController(IBillRespository billRepository, IRoomRespository roomRespository)
        {
            this.billRepository = billRepository;
            this.roomRespository = roomRespository;
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
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string StartTime = HttpContext.Current.Request.Params["StartTime"];
            string EndTime = HttpContext.Current.Request.Params["EndTime"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetRoomBills(RoomNo, RealName, StartTime, EndTime);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetRoomBillsCount(RoomNo, RealName, StartTime, EndTime));
                bills = this.billRepository.GetRoomPagerBills(RoomNo, RealName, StartTime, EndTime, pageIndex, pageSize, u => u.RoomNo, true);
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
            string[] titles = { "用户UUID", "业主姓名", "结算时间", "设备名称", "设备累计读数", "设备结算能耗", 
                                  "设备结算价格", "总读数", "结算总能耗","结算总价格" };
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
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                bills = this.billRepository.GetPrepaidBills(RoomNo, BuildingNo, RealName);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.billRepository.GetPrepaidBillsCount(RoomNo, BuildingNo, RealName));
                bills = this.billRepository.GetPrepaidPagerBills(RoomNo, BuildingNo, RealName, pageIndex, pageSize, u => u.RoomNo);
            }
            pager.Items = bills;

            return Ok(pager);
        }

        [HttpGet]
        [Route("api/export/userprepaidbills")]
        public IHttpActionResult ExportUserPrepaidBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string fileName = "业主能耗缴费实时账单.xls";
            string[] titles = { "业主UUID", "业主姓名", "建筑名称", "房间编号", "设备名称", "上次抄表读数", "当前抄表读数", "当前用能", 
                                  "当前能耗价格","当前总能耗","当前结算总价","账户余额","结算后账户余额","账户报警金额","账户可透支金额" };
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

            string strDeviceEnergies = HttpContext.Current.Request.Params["DeviceEnergies"];
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string CurrentAccountBalance = HttpContext.Current.Request.Params["CurrentAccountBalance"];
            strDeviceEnergies = string.Format("[{0}]", strDeviceEnergies); // 格式化为json数组
            List<PrepaidDeviceBill> bills = JsonConvert.DeserializeObject<List<PrepaidDeviceBill>>(strDeviceEnergies);
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
                await this.billRepository.AddAsync(bill);
            }

            Room room = await this.roomRespository.GetByIdAsync(RoomNo);
            room.AccountBalance = Convert.ToInt32(CurrentAccountBalance);
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