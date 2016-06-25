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
    }
}