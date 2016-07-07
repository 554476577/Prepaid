using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class UserRespository : AbstractRespository<string, User>, IUserRespository
    {
        public override System.Data.Entity.DbSet<User> GetAll()
        {
            return db.Users;
        }

        public override bool IsExist(string uuid)
        {
            return db.Users.Count(e => e.UUID == uuid) > 0;
        }

        public IEnumerable<User> GetAll(string userID, string realName, string buildingName, string roomNo)
        {
            IEnumerable<User> result = GetAll();
            if (!string.IsNullOrEmpty(userID))
                result = result.Where(u => u.UUID.Contains(userID));
            if (!string.IsNullOrEmpty(realName))
                result = result.Where(u => u.RealName != null && u.RealName.Contains(realName));
            if (!string.IsNullOrEmpty(buildingName))
                result = result.Where(u => u.BuildingName != null && u.BuildingName.Contains(buildingName));
            if (!string.IsNullOrEmpty(roomNo))
                result = result.Where(u => u.RoomNo != null && u.RoomNo.Contains(roomNo));
            return result;
        }

        public int GetCount(string userID, string realName, string buildingName, string roomNo)
        {
            return GetAll(userID, realName, buildingName, roomNo).Count();
        }

        public IEnumerable<User> GetPagerItems(string userID, string realName, string buildingName, string roomNo,
            int pageIndex, int pageSize, Func<User, string> func, bool isDesc = false)
        {
            var result = GetAll(userID, realName, buildingName, roomNo);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }
    }
}