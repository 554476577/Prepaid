using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class RechargeRespository : AbstractRespository<string, Recharge>, IRechargeRespository
    {
        public override System.Data.Entity.DbSet<Recharge> GetAll()
        {
            return db.Recharges;
        }

        public override bool IsExist(string uuid)
        {
            return db.Recharges.Count(e => e.UUID == uuid) > 0;
        }

        public IEnumerable<Recharge> GetAll(string RoomNo, string RealName)
        {
            IEnumerable<Recharge> result = GetAll();
            if (!string.IsNullOrEmpty(RoomNo))
                result = result.Where(u => u.RoomNo.Contains(RoomNo));
            if (!string.IsNullOrEmpty(RealName))
                result = result.Where(u => u.Room.RealName != null && u.Room.RealName.Contains(RealName));
            return result;
        }

        public int GetCount(string RoomNo, string RealName)
        {
            return GetAll(RoomNo, RealName).Count();
        }

        public IEnumerable<Recharge> GetPagerItems(string RoomNo, string RealName, int pageIndex, int pageSize, Func<Recharge, DateTime?> func, bool isDesc = false)
        {
            var result = GetAll(RoomNo, RealName);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}