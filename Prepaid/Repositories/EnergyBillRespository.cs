using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class EnergyBillRespository : AbstractRespository<int, EnergyBill>, IEnergyBillRespository
    {
        public override System.Data.Entity.DbSet<EnergyBill> GetAll()
        {
            return db.EnergyBills;
        }

        public override bool IsExist(int uuid)
        {
            return db.EnergyBills.Count(e => e.ID == uuid) > 0;
        }

        public IEnumerable<UserEnergy> GetUserEnergies()
        {
            List<UserEnergy> userEnergies = new List<UserEnergy>();
            var result = from p in db.EnergyBills
                         group p by new
                         {
                             UserID = p.DeviceLink.User.UUID,
                             RealName = p.DeviceLink.User.RealName,
                             DateTime = p.DateTime
                         } into g
                         select new
                         {
                             UserID = g.Key.UserID,
                             RealName = g.Key.RealName,
                             DateTime = g.Key.DateTime,
                             SumTotolValue = g.Sum(p => p.TotolValue),
                             SumValue = g.Sum(p => p.Value),
                             SumMoney = g.Sum(p => p.Money)
                         };
            result = from item in result orderby item.DateTime descending select item;
            foreach (var item in result)
            {
                UserEnergy userEnergy = new UserEnergy();
                userEnergy.UserID = item.UserID;
                userEnergy.RealName = item.RealName;
                userEnergy.DateTime = item.DateTime;
                userEnergy.SumTotolValue = item.SumTotolValue;
                userEnergy.SumValue = item.SumValue;
                userEnergy.SumMoney = TextHelper.ConvertMoney(item.SumMoney);
                userEnergy.DeviceEnergies = (from p in GetAll()
                                             where p.DeviceLink.UserID == item.UserID && p.DateTime == item.DateTime
                                             select p).AsEnumerable().Select(q => new DeviceEnergy
                                             {
                                                 PointID = q.DeviceLink.Point.ID,
                                                 DeviceName = q.DeviceLink.Point.DeviceName,
                                                 TotolValue = q.TotolValue,
                                                 Value = q.Value,
                                                 Money = TextHelper.ConvertMoney(q.Money),
                                                 Remark = q.Remark
                                             });
                userEnergies.Add(userEnergy);
            }

            return userEnergies;
        }

        public IEnumerable<UserEnergy> GetUserPagerEnergies(int pageIndex, int pageSize, Func<UserEnergy, string> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetUserEnergies().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetUserEnergies().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<UserEnergy> GetUserEnergies(string userID, string realName, string startTime, string endTime)
        {
            var result = GetUserEnergies();
            if (!string.IsNullOrEmpty(userID))
                result = result.Where(u => u.UserID.Contains(userID));
            if (!string.IsNullOrEmpty(realName))
                result = result.Where(u => u.RealName != null && u.RealName.Contains(realName));
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime dateTime = Convert.ToDateTime(startTime);
                result = result.Where(u => u.DateTime != null && u.DateTime >= dateTime);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dateTime = Convert.ToDateTime(endTime);
                result = result.Where(u => u.DateTime != null && u.DateTime <= dateTime);
            }
            return result;
        }

        public int GetUserEnergiesCount(string userID, string realName, string startTime, string endTime)
        {
            return GetUserEnergies(userID, realName, startTime, endTime).Count();
        }

        public IEnumerable<UserEnergy> GetUserPagerEnergies(string userID, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<UserEnergy, string> func, bool isDesc = false)
        {
            var result = GetUserEnergies(userID, realName, startTime, endTime);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<PrepaidEnergy> GetPrepaidEnergies()
        {
            List<PrepaidEnergy> prepaidEnergies = new List<PrepaidEnergy>();
            foreach (var item in db.Users)
            {
                PrepaidEnergy prepaidEnergy = new PrepaidEnergy();
                prepaidEnergy.UserID = item.UUID;
                prepaidEnergy.RealName = item.RealName;
                prepaidEnergy.BuildingName = item.BuildingName;
                prepaidEnergy.RoomNo = item.RoomNo;
                prepaidEnergy.AccountBalance = TextHelper.ConvertMoney(item.AccountBalance);
                prepaidEnergy.AccountWarnLimit = TextHelper.ConvertMoney(item.AccountWarnLimit);
                prepaidEnergy.CreditScore = item.CreditScore;
                prepaidEnergy.InstantDeviceEnergies = GetInstantDeviceEnergies(item.UUID);
                prepaidEnergy.CurrentSumValue = prepaidEnergy.InstantDeviceEnergies.Sum(o => o.IntervalValue);
                prepaidEnergy.CurrentSumMoney = prepaidEnergy.InstantDeviceEnergies.Sum(o => o.IntervalMoney);
                prepaidEnergy.StrCurrentSumMoney = TextHelper.ConvertMoney(prepaidEnergy.CurrentSumMoney);
                prepaidEnergy.CurrentAccountBalance = item.AccountBalance - prepaidEnergy.CurrentSumMoney;
                prepaidEnergy.StrCurrentAccountBalance = TextHelper.ConvertMoney(prepaidEnergy.CurrentAccountBalance);
                prepaidEnergies.Add(prepaidEnergy);
            }

            return prepaidEnergies;
        }

        private IEnumerable<InstantDeviceEnergy> GetInstantDeviceEnergies(string userID)
        {
            List<InstantDeviceEnergy> deviceEnergies = new List<InstantDeviceEnergy>();
            var devices = from item in db.DeviceLinks where item.User.UUID == userID select item;
            foreach (var item in devices)
            {
                InstantDeviceEnergy deviceEnergy = new InstantDeviceEnergy();
                deviceEnergy.DeviceLinkID = item.ID;
                deviceEnergy.PointID = item.Point.ID;
                deviceEnergy.DeviceName = item.Point.DeviceName;
                var preEnergy = (from p in db.EnergyBills
                                 where p.DeviceLinkID == item.ID
                                 orderby p.DateTime descending
                                 select p).FirstOrDefault();
                if (preEnergy != null)
                {
                    deviceEnergy.PreDateTime = preEnergy.DateTime;
                    deviceEnergy.PreValue = preEnergy.TotolValue;
                    deviceEnergy.CurrentValue = Convert.ToDouble(item.Point.Value);
                    deviceEnergy.IntervalValue = deviceEnergy.CurrentValue - deviceEnergy.PreValue;
                    deviceEnergy.IntervalMoney = Convert.ToInt32(deviceEnergy.IntervalValue * item.Point.Price);
                    deviceEnergy.StrIntervalMoney = TextHelper.ConvertMoney(deviceEnergy.IntervalMoney);
                }
                deviceEnergies.Add(deviceEnergy);
            }

            return deviceEnergies;
        }

        public IEnumerable<PrepaidEnergy> GetPrepaidPagerEnergies(int pageIndex, int pageSize, Func<PrepaidEnergy, string> func, bool isDesc = false)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetPrepaidEnergies().OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return GetPrepaidEnergies().OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<PrepaidEnergy> GetPrepaidEnergies(string userID, string realName, string buildingName, string roomNo)
        {
            var result = GetPrepaidEnergies();
            if (!string.IsNullOrEmpty(userID))
                result = result.Where(u => u.UserID.Contains(userID));
            if (!string.IsNullOrEmpty(realName))
                result = result.Where(u => u.RealName != null && u.RealName.Contains(realName));
            if (!string.IsNullOrEmpty(buildingName))
                result = result.Where(u => u.BuildingName != null && u.BuildingName.Contains(buildingName));
            if (!string.IsNullOrEmpty(roomNo))
                result = result.Where(u => u.RoomNo != null && u.RoomNo.Contains(roomNo));
            return result;
        }

        public int GetPrepaidEnergiesCount(string userID, string realName, string buildingName, string roomNo)
        {
            return GetPrepaidEnergies(userID, realName, buildingName, roomNo).Count();
        }

        public IEnumerable<PrepaidEnergy> GetPrepaidPagerEnergies(string userID, string realName, string buildingName, string roomNo,
            int pageIndex, int pageSize, Func<PrepaidEnergy, string> func, bool isDesc = false)
        {
            var result = GetPrepaidEnergies(userID, realName, buildingName, roomNo);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}