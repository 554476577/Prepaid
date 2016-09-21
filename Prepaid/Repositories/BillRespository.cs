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
                             RealName = p.Device.Room.RealName,
                             LotNo = p.LotNo,
                             AccountBalance = p.AccountBalance,
                             DateTime = p.DateTime,
                         } into g
                         orderby g.Key.RoomNo, g.Key.DateTime descending
                         select new
                         {
                             RoomNo = g.Key.RoomNo,
                             RealName = g.Key.RealName,
                             LotNo = g.Key.LotNo,
                             AccountBalance = g.Key.AccountBalance,
                             DateTime = g.Key.DateTime,
                             //SumValue = g.Sum(p => p.CurValue - p.PreValue),
                             SumMoney = g.Sum(p => p.Money)
                         };
            foreach (var item in result)
            {
                RoomBill bill = new RoomBill();
                bill.RoomNo = item.RoomNo;
                bill.LotNo = item.LotNo;
                bill.RealName = item.RealName;
                bill.DateTime = item.DateTime;
                bill.AccountBalance = TextHelper.ConvertMoney(item.AccountBalance);
                //bill.SumValue = item.SumValue;
                bill.SumMoney = TextHelper.ConvertMoney(item.SumMoney);
                bill.BilledAccountBalance = TextHelper.ConvertMoney(item.AccountBalance - item.SumMoney);
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

        public IEnumerable<RoomBill> GetRoomBills<T>(int pageIndex, int pageSize, Func<RoomBill, T> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetRoomBills().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetRoomBills().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<RoomBill> GetRoomBills(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime)
        {
            IEnumerable<Bill> bills = db.Bills;
            if (!string.IsNullOrEmpty(roomNo))
                bills = bills.Where(u => u.Device.RoomNo.Contains(roomNo));
            if (!string.IsNullOrEmpty(buildingNo))
                bills = bills.Where(u => u.Device.Room.BuildingNo.Contains(buildingNo));
            if (!string.IsNullOrEmpty(floor))
                bills = bills.Where(u => u.Device.Room.Floor == Convert.ToInt32(floor));
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

        public int GetRoomBillsCount(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime)
        {
            return GetRoomBills(roomNo, buildingNo, floor, realName, startTime, endTime).Count();
        }

        public IEnumerable<RoomBill> GetRoomPagerBills<T>(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<RoomBill, T> func, bool isDesc = false)
        {
            var result = GetRoomBills(roomNo, buildingNo, floor, realName, startTime, endTime);
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
                var credit = (from p in db.Credits where item.CreditScore > p.MinScore && item.CreditScore <= p.MaxScore select p).FirstOrDefault();
                PrepaidBill bill = new PrepaidBill();
                bill.RoomNo = item.RoomNo;
                bill.BuildingNo = item.BuildingNo;
                bill.RealName = item.RealName;
                bill.Phone = item.Phone;
                bill.CreditScore = item.CreditScore;
                bill.CreditLevel = credit.Name;
                bill.IntAccountBalance = item.AccountBalance;
                bill.AccountBalance = TextHelper.ConvertMoney(item.AccountBalance);
                bill.Arrears = TextHelper.ConvertMoney(credit.Arrears);
                bill.ManagerFees = string.Format("{0}㎡*￥{1}={2}",
                    item.Area, TextHelper.ConvertMoney(item.Price), TextHelper.ConvertMoney((int)item.Area * item.Price));
                //bill.IntApportMoney = GetApportMoney(item.RoomNo);
                //bill.ApportMoney = TextHelper.ConvertMoney(bill.IntApportMoney);
                bill.PrepaidDeviceBills = GetPrepaidDeviceBills(item.RoomNo);
                //bill.SumValue = bill.PrepaidDeviceBills.Sum(o => o.CurValue - o.PreValue);
                //bill.SumValue = Math.Round(bill.SumValue ?? 0.00, 2);
                bill.IntSumMoney = bill.PrepaidDeviceBills.Sum(o => o.IntMoney);
                bill.SumMoney = TextHelper.ConvertMoney(bill.IntSumMoney);
                //bill.IntBilledBalance = item.AccountBalance - bill.IntSumMoney - bill.IntApportMoney;
                bill.IntBilledBalance = item.AccountBalance - bill.IntSumMoney;
                bill.BilledBalance = TextHelper.ConvertMoney(bill.IntBilledBalance);
                bills.Add(bill);
            }

            return bills;
        }

        ///// <summary>
        ///// 获取指定房间需要分摊的费用
        ///// </summary>
        ///// <param name="roomNo"></param>
        ///// <returns></returns>
        //private int? GetApportMoney(string roomNo)
        //{
        //    int? totalMoney = 0;
        //    IEnumerable<VRoomPay> roomPays = from p in db.VRoomPays where p.RoomNo == roomNo select p;
        //    foreach (VRoomPay item in roomPays)
        //    {
        //        double? money = (item.Value - item.PreValue) * item.Price1;
        //        money = (item.Area / item.TotolArea) * money;
        //        totalMoney += (int)money;
        //    }

        //    return totalMoney;
        //}

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

        public IEnumerable<PrepaidBill> GetPrepaidPagerBills<T>(int pageIndex, int pageSize, Func<PrepaidBill, T> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetPrepaidBills().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetPrepaidBills().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<PrepaidBill> GetPrepaidBills(string roomNo, string buildingNo, string floor, string realName)
        {
            IEnumerable<Room> rooms = db.Rooms;
            if (!string.IsNullOrEmpty(roomNo))
                rooms = rooms.Where(u => u.RoomNo.Contains(roomNo));
            if (!string.IsNullOrEmpty(buildingNo))
                rooms = rooms.Where(u => u.BuildingNo != null && u.BuildingNo.Contains(buildingNo));
            if (!string.IsNullOrEmpty(floor))
                rooms = rooms.Where(u => u.Floor == Convert.ToInt32(floor));
            if (!string.IsNullOrEmpty(realName))
                rooms = rooms.Where(u => u.RealName != null && u.RealName.Contains(realName));

            return GetPrepaidBills(rooms);
        }

        public int GetPrepaidBillsCount(string roomNo, string buildingNo, string floor, string realName)
        {
            return GetPrepaidBills(roomNo, buildingNo, floor, realName).Count();
        }

        public IEnumerable<PrepaidBill> GetPrepaidPagerBills<T>(string roomNo, string buildingNo, string floor, string realName,
            int pageIndex, int pageSize, Func<PrepaidBill, T> func, bool isDesc = false)
        {
            var result = GetPrepaidBills(roomNo, buildingNo, floor, realName);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}