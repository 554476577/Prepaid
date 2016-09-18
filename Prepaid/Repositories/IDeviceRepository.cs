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
        IEnumerable<Device> GetAll(string deviceNo, string roomNo, string itemID);

        int GetCount(string deviceNo, string roomNo, string itemID);

        IEnumerable<Device> GetPagerItems(string deviceNo, string roomNo, string itemID,
            int pageIndex, int pageSize, Func<Device, string> func, bool isDesc = false);

        IEnumerable<Statis> GetBuildingStatisInfo();

        IEnumerable<Statis> GetTypeStatisInfo();

        IEnumerable<Statis> GetBuildingTypeStatisInfo(string buildingNo);

        IEnumerable<VBuildEp> GetDayEp();

        IEnumerable<VBuildEp> GetMonthEp();

        IEnumerable<Statis> GetBuildingDayEp(string buildingNo);

        IEnumerable<Statis> GetBuildingMonthEp(string buildingNo);

        IEnumerable<dynamic> GetRealtimeFunds();

        Task<int> BatchImport(string fullName, bool isDeleteAll);
    }
}