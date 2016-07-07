using Prepaid.Models;
using Prepaid.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IPointRepository : IRepository<int, Point>
    {
        Point GetByPointID(string pointID);

        IEnumerable<Point> GetOriginalAll();

        int GetOriginalCount();

        IEnumerable<Point> GetOriginalAll(string pointID, string deviceName, string itemID);

        int GetOriginalCount(string pointID, string deviceName, string itemID);

        IEnumerable<Point> GetOriginalPagerItems(string pointID, string deviceName, string itemID,
            int pageIndex, int pageSize, Func<Point, string> func, bool isDesc = false);
    }
}