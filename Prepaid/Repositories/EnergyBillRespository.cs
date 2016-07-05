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
                userEnergy.SumMoney = item.SumMoney;
                userEnergy.DeviceEnergies = from p in GetAll()
                                            where p.DeviceLink.UserID == item.UserID && p.DateTime==item.DateTime
                                            select new DeviceEnergy
                                            {
                                                PointID = p.DeviceLink.Point.ID,
                                                DeviceName = p.DeviceLink.Point.DeviceName,
                                                TotolValue = p.TotolValue,
                                                Value = p.Value,
                                                Money = p.Money,
                                                Remark = p.Remark
                                            };
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
                prepaidEnergy.AccountBalance = item.AccountBalance;
                prepaidEnergy.AccountWarnLimit = item.AccountWarnLimit;
                prepaidEnergy.CreditScore = item.CreditScore;
                prepaidEnergy.InstantDeviceEnergies = GetInstantDeviceEnergies(item.UUID);
                prepaidEnergy.CurrentSumValue = prepaidEnergy.InstantDeviceEnergies.Sum(o => o.IntervalValue);
                prepaidEnergy.CurrentSumMoney = prepaidEnergy.InstantDeviceEnergies.Sum(o => o.IntervalMoney);
                prepaidEnergy.CurrentAccountBalance = prepaidEnergy.AccountBalance - prepaidEnergy.CurrentSumMoney;
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
                    deviceEnergy.IntervalMoney = Convert.ToInt32(deviceEnergy.IntervalValue * 1.05);
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
    }
}