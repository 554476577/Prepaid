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
                                            where p.DeviceLink.UserID == item.UserID
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
    }
}