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

        public IEnumerable<Recharge> GetAll(string userID, string realName)
        {
            IEnumerable<Recharge> result = GetAll();
            if (!string.IsNullOrEmpty(userID))
                result = result.Where(u => u.UserID.Contains(userID));
            if (!string.IsNullOrEmpty(realName))
                result = result.Where(u => u.User.RealName != null && u.User.RealName.Contains(realName));
            return result;
        }

        public int GetCount(string userID, string realName)
        {
            return GetAll(userID, realName).Count();
        }

        public IEnumerable<Recharge> GetPagerItems(string userID, string realName, int pageIndex, int pageSize, Func<Recharge, string> func, bool isDesc = false)
        {
            var result = GetAll(userID, realName);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}