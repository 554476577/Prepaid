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

        public IEnumerable<Room> GetAll(string RealName, string BuildingNo, string RoomNo)
        {
            IEnumerable<Room> result = GetAll();
            if (!string.IsNullOrEmpty(RealName))
                result = result.Where(u => u.RealName != null && u.RealName.Contains(RealName));
            if (!string.IsNullOrEmpty(BuildingNo))
                result = result.Where(u => u.BuildingNo != null && u.BuildingNo.Contains(BuildingNo));
            if (!string.IsNullOrEmpty(RoomNo))
                result = result.Where(u => u.RoomNo != null && u.RoomNo.Contains(RoomNo));
            return result;
        }

        public int GetCount(string RealName, string BuildingNo, string RoomNo)
        {
            return GetAll(RealName, BuildingNo, RoomNo).Count();
        }

        public IEnumerable<Room> GetPagerItems(string RealName, string BuildingNo, string RoomNo,
            int pageIndex, int pageSize, Func<Room, string> func, bool isDesc = false)
        {
            var result = GetAll(RealName, BuildingNo, RoomNo);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}