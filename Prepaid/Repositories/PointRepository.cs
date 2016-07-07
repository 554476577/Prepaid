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

        public IEnumerable<Point> GetOriginalAll(string pointID, string deviceName, string itemID)
        {
            var result = GetOriginalAll();
            if (!string.IsNullOrEmpty(pointID))
                result = result.Where(u => u.PointID.Contains(pointID));
            if (!string.IsNullOrEmpty(deviceName))
                result = result.Where(u => u.DeviceName != null && u.DeviceName.Contains(deviceName));
            if (!string.IsNullOrEmpty(itemID))
                result = result.Where(u => u.ItemID.Contains(itemID));
            return result;
        }

        public int GetOriginalCount(string pointID, string deviceName, string itemID)
        {
            return GetOriginalAll(pointID, deviceName, itemID).Count();
        }

        public IEnumerable<Point> GetOriginalPagerItems(string pointID, string deviceName, string itemID,
            int pageIndex, int pageSize, Func<Point, string> func, bool isDesc = false)
        {
            var result = GetOriginalAll(pointID, deviceName, itemID);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}