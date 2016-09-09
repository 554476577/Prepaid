using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<int> BatchImport(string fullName, bool isDeleteAll)
        {
            int rowAffected = 0;
            if (isDeleteAll)
            {
                GetAll().RemoveRange(GetAll());
                rowAffected += await db.SaveChangesAsync();
            }

            string text = string.Empty;
            using (StreamReader reader = new StreamReader(fullName, Encoding.GetEncoding("GB2312")))
            {
                reader.ReadLine(); // first line ignored!
                string line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    int index = 0;
                    Room room = new Room();
                    string[] data = line.Split(',');
                    room.RoomNo = data[index++];
                    room.BuildingNo = data[index++];
                    room.Floor = Convert.ToInt32(data[index++]);
                    room.Area = Convert.ToDouble(data[index++]);
                    room.Price = Convert.ToInt32(data[index++]);
                    room.RealName = data[index++];
                    room.Phone = data[index++];
                    room.AccountBalance = Convert.ToInt32(data[index++]);
                    room.CreditScore = Convert.ToInt32(data[index++]);
                    GetAll().Add(room);
                    line = reader.ReadLine();
                }
            }
            rowAffected += await db.SaveChangesAsync();

            return rowAffected;
        }
    }
}