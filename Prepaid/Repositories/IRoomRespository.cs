using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Prepaid.Repositories
{
    public interface IRoomRespository : IRepository<string, Room>
    {
        IEnumerable<Room> GetAll(string RealName, string BuildingNo, string RoomNo, string Floor);

        int GetCount(string RealName, string BuildingNo, string RoomNo, string Floor);

        IEnumerable<Room> GetPagerItems(string RealName, string BuildingNo, string RoomNo, string Floor,
            int pageIndex, int pageSize, Func<Room, string> func, bool isDesc = false);

        Task<int> BatchImport(string fullName, bool isDeleteAll);
    }
}