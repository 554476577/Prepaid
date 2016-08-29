﻿using Prepaid.Models;
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
                return GetOriginalAll().OrderBy(orderFunc).Skip(recordStart).Take(pageSize);
            else
                return GetOriginalAll().OrderByDescending(orderFunc).Skip(recordStart).Take(pageSize);
        }

        public IEnumerable<Device> GetOriginalAll()
        {
            return GetAll().Where(u => (u.ArchiveTag ?? false) == false);
        }

        public int GetOriginalCount()
        {
            return GetOriginalAll().Count();
        }

        public IEnumerable<Device> GetOriginalAll(string deviceNo, string roomNo, string itemID)
        {
            var result = GetOriginalAll();
            if (!string.IsNullOrEmpty(deviceNo))
                result = result.Where(u => u.DeviceNo.Contains(deviceNo));
            if (!string.IsNullOrEmpty(roomNo))
                result = result.Where(u => u.RoomNo != null && u.DeviceName.Contains(roomNo));
            if (!string.IsNullOrEmpty(itemID))
                result = result.Where(u => u.ItemID.Contains(itemID));
            return result;
        }

        public int GetOriginalCount(string deviceNo, string roomNo, string itemID)
        {
            return GetOriginalAll(deviceNo, roomNo, itemID).Count();
        }

        public IEnumerable<Device> GetOriginalPagerItems(string deviceNo, string roomNo, string itemID,
            int pageIndex, int pageSize, Func<Device, string> func, bool isDesc = false)
        {
            var result = GetOriginalAll(deviceNo, roomNo, itemID);
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
                             yAxis = (from q in db.Devices where q.Room.BuildingNo == g.Key.BuildingNo select q).Sum(a => a.Value - a.PreValue)
                         };

            return result;
        }

        public IEnumerable<Statis> GetTypeStatisInfo()
        {
            var result = from p in db.DeviceTypes
                         group p by new { TypeID = p.UUID, TypeName = p.Name } into g
                         orderby g.Key.TypeName
                         select new Statis
                         {
                             xAxis = g.Key.TypeName,
                             yAxis = (from q in db.Devices where q.TypeID == g.Key.TypeID select q).Sum(a => a.Value - a.PreValue)
                         };

            return result;
        }
    }
}