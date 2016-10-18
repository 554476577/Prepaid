using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface ILogRepository : IRepository<int, Log>
    {
        IEnumerable<Log> GetAll(string userName, string type, string startTime, string endTime);

        int GetCount(string userName, string type, string startTime, string endTime);

        IEnumerable<Log> GetPagerItems<T>(string userName, string type, string startTime, string endTime,
            int pageIndex, int pageSize, Func<Log, T> func, bool isDesc = false);
    }
}