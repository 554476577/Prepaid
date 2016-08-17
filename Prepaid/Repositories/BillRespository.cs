using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class BillRespository : AbstractRespository<int, Bill>, IBillRespository
    {
        public override System.Data.Entity.DbSet<Bill> GetAll()
        {
            return db.Bills;
        }

        public override bool IsExist(int uuid)
        {
            return db.Bills.Count(e => e.ID == uuid) > 0;
        }

        private IEnumerable<RoomBill> GetRoomBills(IEnumerable<Bill> srcBills)
        {
            List<RoomBill> bills = new List<RoomBill>();
            var result = from p in srcBills
                         group p by new
                         {
                             RoomNo = p.Device.RoomNo,
                             LotNo = p.LotNo,
                             RealName = p.Device.Room.RealName,
                             DateTime = p.DateTime
                         } into g
                         orderby g.Key.RoomNo, g.Key.LotNo
                         select new
                         {
                             RoomNo = g.Key.RoomNo,
                             LotNo = g.Key.LotNo,
                             RealName = g.Key.RealName,
                             DateTime = g.Key.DateTime,
                             SumValue = g.Sum(p => p.CurValue - p.PreValue),
                             SumMoney = g.Sum(p => p.Money)
                         };
            foreach (var item in result)
            {
                RoomBill bill = new RoomBill();
                bill.RoomNo = item.RoomNo;
                bill.LotNo = item.LotNo;
                bill.RealName = item.RealName;
                bill.DateTime = item.DateTime;
                bill.SumValue = item.SumValue;
                bill.SumMoney = TextHelper.ConvertMoney(item.SumMoney);
                bill.DeviceBills = (from p in GetAll()
                                    where p.Device.RoomNo == item.RoomNo && p.LotNo == item.LotNo
                                    select p).AsEnumerable().Select(q => new DeviceBill
                                    {
                                        DeviceNo = q.DeviceNo,
                                        DeviceName = q.Device.DeviceName,
                                        PreValue = q.PreValue,
                                        CurValue = q.CurValue,
                                        Price = TextHelper.ConvertMoney(q.Device.DeviceType.Price1),
                                        Money = TextHelper.ConvertMoney(q.Money),
                                        Remark = q.Remark
                                    });
                bills.Add(bill);
            }

            return bills;
        }

        public IEnumerable<RoomBill> GetRoomBills()
        {
            return GetRoomBills(GetAll());
        }

        public IEnumerable<RoomBill> GetRoomBills(int pageIndex, int pageSize, Func<RoomBill, string> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetRoomBills().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetRoomBills().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<RoomBill> GetRoomBills(string roomNo, string realName, string startTime, string endTime)
        {
            IEnumerable<Bill> bills = db.Bills;
            if (!string.IsNullOrEmpty(roomNo))
                bills = bills.Where(u => u.Device.RoomNo.Contains(roomNo));
            if (!string.IsNullOrEmpty(realName))
                bills = bills.Where(u => u.Device.Room.RealName != null && u.Device.Room.RealName.Contains(realName));
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dateTime = Convert.ToDateTime(startTime);
                bills = bills.Where(u => u.DateTime != null && u.DateTime >= dateTime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dateTime = Convert.ToDateTime(endTime);
                bills = bills.Where(u => u.DateTime != null && u.DateTime <= dateTime);
            }

            return GetRoomBills(bills);
        }

        public int GetRoomBillsCount(string roomNo, string realName, string startTime, string endTime)
        {
            return GetRoomBills(roomNo, realName, startTime, endTime).Count();
        }

        public IEnumerable<RoomBill> GetRoomPagerBills(string roomNo, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<RoomBill, string> func, bool isDesc = false)
        {
            var result = GetRoomBills(roomNo, realName, startTime, endTime);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        private IEnumerable<PrepaidBill> GetPrepaidBills(IEnumerable<Room> rooms)
        {
            List<PrepaidBill> bills = new List<PrepaidBill>();
            foreach (var item in rooms)
            {
                PrepaidBill bill = new PrepaidBill();
                bill.RoomNo = item.RoomNo;
                bill.BuildingNo = item.BuildingNo;
                bill.RealName = item.RealName;
                bill.IntAccountBalance = item.AccountBalance;
                bill.AccountBalance = TextHelper.ConvertMoney(item.AccountBalance);
                bill.PrepaidDeviceBills = GetPrepaidDeviceBills(item.RoomNo);
                bill.SumValue = bill.PrepaidDeviceBills.Sum(o => o.CurValue - o.PreValue);
                bill.IntSumMoney = bill.PrepaidDeviceBills.Sum(o => o.IntMoney);
                bill.SumMoney = TextHelper.ConvertMoney(bill.IntSumMoney);
                bills.Add(bill);
            }

            return bills;
        }

        public IEnumerable<PrepaidBill> GetPrepaidBills()
        {
            return GetPrepaidBills(db.Rooms);
        }

        private IEnumerable<PrepaidDeviceBill> GetPrepaidDeviceBills(string RoomNo)
        {
            List<PrepaidDeviceBill> bills = new List<PrepaidDeviceBill>();
            var devices = from item in db.Devices where item.RoomNo == RoomNo select item;
            foreach (var item in devices)
            {
                PrepaidDeviceBill bill = new PrepaidDeviceBill();
                bill.DeviceNo = item.DeviceNo;
                bill.DeviceName = item.DeviceName;
                bill.PreValue = Math.Round(item.PreValue ?? 0.00, 2);
                bill.CurValue = Math.Round((item.Value * item.Rate) ?? 0.00, 2);
                bill.Price = TextHelper.ConvertMoney(item.DeviceType.Price1);
                bill.IntMoney = (int)((bill.CurValue - bill.PreValue) * item.DeviceType.Price1);
                bill.Money = TextHelper.ConvertMoney(bill.IntMoney);
                bills.Add(bill);
            }

            return bills;
        }

        public IEnumerable<PrepaidBill> GetPrepaidPagerBills(int pageIndex, int pageSize, Func<PrepaidBill, string> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetPrepaidBills().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetPrepaidBills().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<PrepaidBill> GetPrepaidBills(string roomNo, string buildingNo, string realName)
        {
            IEnumerable<Room> rooms = db.Rooms;
            if (!string.IsNullOrEmpty(roomNo))
                rooms = rooms.Where(u => u.RoomNo.Contains(roomNo));
            if (!string.IsNullOrEmpty(buildingNo))
                rooms = rooms.Where(u => u.BuildingNo != null && u.BuildingNo.Contains(buildingNo));
            if (!string.IsNullOrEmpty(realName))
                rooms = rooms.Where(u => u.RealName != null && u.RealName.Contains(realName));

            return GetPrepaidBills(rooms);
        }

        public int GetPrepaidBillsCount(string roomNo, string buildingNo, string realName)
        {
            return GetPrepaidBills(roomNo, buildingNo, realName).Count();
        }

        public IEnumerable<PrepaidBill> GetPrepaidPagerBills(string roomNo, string buildingNo, string realName,
            int pageIndex, int pageSize, Func<PrepaidBill, string> func, bool isDesc = false)
        {
            var result = GetPrepaidBills(roomNo, buildingNo, realName);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}