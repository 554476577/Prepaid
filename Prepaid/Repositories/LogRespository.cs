using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class LogRespository : AbstractRespository<int, Log>, ILogRepository
    {
        public override System.Data.Entity.DbSet<Log> GetAll()
        {
            return db.Logs;
        }

        public override bool IsExist(int uuid)
        {
            return db.Logs.Count(e => e.ID == uuid) > 0;
        }

        public IEnumerable<Log> GetAll(string userName, string type, string startTime, string endTime)
        {
            IEnumerable<Log> result = GetAll();
            if (!string.IsNullOrEmpty(userName))
                result = result.Where(u => u.Admin.UserName.Contains(userName));
            if (!string.IsNullOrEmpty(type))
            {
                int intType = Convert.ToInt32(type);
                result = result.Where(u => u.Type == intType);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime time = Convert.ToDateTime(startTime);
                result = result.Where(u => u.DateTime >= time);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime time = Convert.ToDateTime(endTime);
                result = result.Where(u => u.DateTime <= time);
            }

            return result;
        }

        public int GetCount(string userName, string type, string startTime, string endTime)
        {
            return GetAll(userName, type, startTime, endTime).Count();
        }

        public IEnumerable<Log> GetPagerItems<T>(string userName, string type, string startTime, string endTime,
            int pageIndex, int pageSize, Func<Log, T> func, bool isDesc = false)
        {
            var result = GetAll(userName, type, startTime, endTime);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}