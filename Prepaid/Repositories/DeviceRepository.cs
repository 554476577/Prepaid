using Prepaid.Models;
using Prepaid.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Prepaid.Repositories
{
    public class DeviceRepository : AbstractRespository<string, Device>, IDeviceRepository
    {
        public override DbSet<Device> GetAll()
        {
            return db.Devices;
        }

        public override bool IsExist(string uuid)
        {
            return db.Devices.Count(e => e.DeviceNo == uuid) > 0;
        }

        public override IEnumerable<Device> GetPagerItems<TKey>(int pageIndex, int pageSize, Func<Device, TKey> orderFunc, bool isDesc)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetAll().OrderBy(orderFunc).Skip(recordStart).Take(pageSize);
            else
                return GetAll().OrderByDescending(orderFunc).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<Device> GetAll(string deviceNo, string roomNo, string itemID)
        {
            IEnumerable<Device> result = GetAll();
            if (!string.IsNullOrEmpty(deviceNo))
                result = result.Where(u => u.DeviceNo.Contains(deviceNo));
            if (!string.IsNullOrEmpty(roomNo))
                result = result.Where(u => u.RoomNo != null && u.RoomNo.Contains(roomNo));
            if (!string.IsNullOrEmpty(itemID))
                result = result.Where(u => u.ItemID.Contains(itemID));
            return result;
        }

        public int GetCount(string deviceNo, string roomNo, string itemID)
        {
            return GetAll(deviceNo, roomNo, itemID).Count();
        }

        public IEnumerable<Device> GetPagerItems(string deviceNo, string roomNo, string itemID,
            int pageIndex, int pageSize, Func<Device, string> func, bool isDesc = false)
        {
            var result = GetAll(deviceNo, roomNo, itemID);
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return result.OrderBy(func).Skip(recordStart).Take(pageSize);
            else
                return result.OrderByDescending(func).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<Statis> GetBuildingStatisInfo()
        {
            var result = from p in db.Buildings
                         group p by new { BuildingNo = p.BuildingNo } into g
                         orderby g.Key.BuildingNo
                         select new Statis
                         {
                             xAxis = g.Key.BuildingNo,
                             yAxis = (from q in db.Devices where q.Room.BuildingNo == g.Key.BuildingNo select q).Sum(a => a.Value - (a.PreValue ?? 0.00))
                         };

            return result;
        }

        public IEnumerable<Statis> GetTypeStatisInfo()
        {
            var result = from p in db.DeviceTypes
                     orderby p.Name
                     select new Statis
                     {
                         xAxis=p.Name,
                         yAxis = (from q in db.Devices where q.TypeID == p.UUID select q).Sum(a => a.Value - (a.PreValue ?? 0.00))
                     };

            return result;
        }

        public IEnumerable<Statis> GetBuildingTypeStatisInfo(string buildingNo)
        {
            var result = from p in db.DeviceTypes
                         orderby p.Name
                         select new Statis
                         {
                             xAxis = p.Name,
                             yAxis = (from q in db.Devices where q.TypeID == p.UUID && q.Room.BuildingNo == buildingNo select q)
                             .Sum(a => a.Value - (a.PreValue ?? 0.00))
                         };

            return result;
        }

        public IEnumerable<VBuildEp> GetDayEp()
        {
            var result = db.Database.SqlQuery<VBuildEp>("SELECT * FROM VBuildDayEp");
            return result;
        }

        public IEnumerable<VBuildEp> GetMonthEp()
        {
            var result = db.Database.SqlQuery<VBuildEp>("SELECT * FROM VBuildMonthEp");
            return result;
        }

        public IEnumerable<Statis> GetBuildingDayEp(string buildingNo)
        {
            var result = from p in GetDayEp()
                         where p.BuildingNo == buildingNo
                         select new Statis
                         {
                             xAxis = p.DateTime,
                             yAxis = p.Value
                         };
            return result;
        }

        public IEnumerable<Statis> GetBuildingMonthEp(string buildingNo)
        {
            var result = from p in GetMonthEp()
                         where p.BuildingNo == buildingNo
                         select new Statis
                         {
                             xAxis = p.DateTime,
                             yAxis = p.Value
                         };
            return result;
        }

        public async Task<int> BatchImport(string fullName, bool isDeleteAll)
        {
            if (!File.Exists(fullName))
                throw new FileNotFoundException();

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
                    Device device = new Device();
                    string[] data = line.Split(',');
                    device.DeviceNo = data[index++];
                    device.RoomNo = data[index++];
                    device.TypeID = data[index++];
                    device.Protocol = data[index++];
                    device.Scope = data[index++];
                    device.DeviceName = data[index++];
                    device.PhyAddr = data[index++];
                    device.ItemID = data[index++];
                    device.ItemName = data[index++];
                    device.ItemDescription = data[index++];
                    device.Rate = Convert.ToDouble(data[index++]);
                    device.DateTime = DateTime.Now;
                    device.IsArchive = Convert.ToBoolean(data[index++]);
                    device.ArchiveInterval = Convert.ToInt32(data[index++]);
                    device.ArchiveTime = DateTime.Now;
                    device.Remark1 = data[index++];
                    device.Remark2 = data[index++];
                    device.Remark3 = data[index++];
                    GetAll().Add(device);
                    line = reader.ReadLine();
                }
            }
            rowAffected += await db.SaveChangesAsync();
            File.Delete(fullName);

            return rowAffected;
        }
    }
}