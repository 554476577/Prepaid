using Prepaid.Models;
using Prepaid.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Prepaid.Repositories
{
    public class PointRepository : AbstractRespository<int, Point>, IPointRepository
    {
        public Point GetByPointID(string pointID)
        {
            return (from item in GetAll() where item.PointID == pointID select item).FirstOrDefault();
        }

        public override DbSet<Point> GetAll()
        {
            return db.Points;
        }

        public override bool IsExist(int uuid)
        {
            return db.Points.Count(e => e.ID == uuid) > 0;
        }

        public override IEnumerable<Point> GetPagerItems<TKey>(int pageIndex, int pageSize, Func<Point, TKey> orderFunc, bool isDesc)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetOriginalAll().OrderBy(orderFunc).Skip(recordStart).Take(pageSize);
            else
                return GetOriginalAll().OrderByDescending(orderFunc).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<Point> GetOriginalAll()
        {
            return GetAll().Where(u => (u.ArchiveTag ?? false) == false);
        }

        public int GetOriginalCount()
        {
            return GetOriginalAll().Count();
        }
    }
}