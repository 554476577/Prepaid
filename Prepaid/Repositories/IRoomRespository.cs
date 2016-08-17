using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public interface IRoomRespository : IRepository<string, Room>
    {
        IEnumerable<Room> GetAll(string RealName, string BuildingNo, string RoomNo);

        int GetCount(string RealName, string BuildingNo, string RoomNo);

        IEnumerable<Room> GetPagerItems(string RealName, string BuildingNo, string RoomNo,
            int pageIndex, int pageSize, Func<Room, string> func, bool isDesc = false);
    }
}