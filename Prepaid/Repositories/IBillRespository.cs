using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IBillRespository : IRepository<int, Bill>
    {
        IEnumerable<RoomBill> GetRoomBills();

        IEnumerable<RoomBill> GetRoomBills(int pageIndex, int pageSize, Func<RoomBill, string> func, bool isDesc = false);

        IEnumerable<RoomBill> GetRoomBills(string roomNo, string realName, string startTime, string endTime);

        int GetRoomBillsCount(string roomNo, string realName, string startTime, string endTime);

        IEnumerable<RoomBill> GetRoomPagerBills(string roomNo, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<RoomBill, string> func, bool isDesc = false);

        IEnumerable<PrepaidBill> GetPrepaidBills();

        IEnumerable<PrepaidBill> GetPrepaidPagerBills(int pageIndex, int pageSize, Func<PrepaidBill, string> func, bool isDesc = false);

        IEnumerable<PrepaidBill> GetPrepaidBills(string roomNo, string buildingNo, string realName);

        int GetPrepaidBillsCount(string roomNo, string buildingNo, string realName);

        IEnumerable<PrepaidBill> GetPrepaidPagerBills(string roomNo, string buildingNo, string realName,
            int pageIndex, int pageSize, Func<PrepaidBill, string> func, bool isDesc = false);
    }
}