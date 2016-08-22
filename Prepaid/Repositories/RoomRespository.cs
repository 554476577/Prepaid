using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class RoomRespository : AbstractRespository<string, Room>, IRoomRespository
    {
        public override System.Data.Entity.DbSet<Room> GetAll()
        {
            return db.Rooms;
        }

        public override bool IsExist(string uuid)
        {
            return db.Rooms.Count(e => e.RoomNo == uuid) > 0;
        }

        public IEnumerable<Room> GetAll(string RealName, string BuildingNo, string RoomNo, string Floor)
        {
            IEnumerable<Room> result = GetAll();
            if (!string.IsNullOrEmpty(RealName))
                result = result.Where(u => u.RealName != null && u.RealName.Contains(RealName));
            if (!string.IsNullOrEmpty(BuildingNo))
                result = result.Where(u => u.BuildingNo != null && u.BuildingNo.Contains(BuildingNo));
            if (!string.IsNullOrEmpty(RoomNo))
                result = result.Where(u => u.RoomNo != null && u.RoomNo.Contains(RoomNo));
            if (!string.IsNullOrEmpty(Floor))
                result = result.Where(u => u.Floor == Convert.ToInt32(Floor));

            return result;
        }

        public int GetCount(string RealName, string BuildingNo, string RoomNo, string Floor)
        {
            return GetAll(RealName, BuildingNo, RoomNo, Floor).Count();
        }

        public IEnumerable<Room> GetPagerItems(string RealName, string BuildingNo, string RoomNo, string Floor,
            int pageIndex, int pageSize, Func<Room, string> func, bool isDesc = false)
        {
            var result = GetAll(RealName, BuildingNo, RoomNo, Floor);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}