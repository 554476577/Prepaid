using Prepaid.Models;
using Prepaid.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IDeviceRepository : IRepository<string, Device>
    {
        IEnumerable<Device> GetOriginalAll();

        int GetOriginalCount();

        IEnumerable<Device> GetOriginalAll(string deviceNo, string roomNo, string itemID);

        int GetOriginalCount(string deviceNo, string roomNo, string itemID);

        IEnumerable<Device> GetOriginalPagerItems(string deviceNo, string roomNo, string itemID,
            int pageIndex, int pageSize, Func<Device, string> func, bool isDesc = false);
    }
}