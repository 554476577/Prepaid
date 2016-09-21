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

        IEnumerable<RoomBill> GetRoomBills<T>(int pageIndex, int pageSize, Func<RoomBill, T> func, bool isDesc = false);

        IEnumerable<RoomBill> GetRoomBills(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime);

        int GetRoomBillsCount(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime);

        IEnumerable<RoomBill> GetRoomPagerBills<T>(string roomNo, string buildingNo, string floor, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<RoomBill, T> func, bool isDesc = false);

        IEnumerable<PrepaidBill> GetPrepaidBills();

        IEnumerable<PrepaidBill> GetPrepaidPagerBills<T>(int pageIndex, int pageSize, Func<PrepaidBill, T> func, bool isDesc = false);

        IEnumerable<PrepaidBill> GetPrepaidBills(string roomNo, string buildingNo, string floor, string realName);

        int GetPrepaidBillsCount(string roomNo, string buildingNo, string floor, string realName);

        IEnumerable<PrepaidBill> GetPrepaidPagerBills<T>(string roomNo, string floor, string buildingNo, string realName,
            int pageIndex, int pageSize, Func<PrepaidBill, T> func, bool isDesc = false);

        IEnumerable<PrepaidBill> GetRecommendPrepaidBills(string buildingNo);

        int GetRecommendPrepaidBillsCount(string buildingNo);

        IEnumerable<PrepaidBill> GetRecommendPrepaidPagerBills(string buildingNo, int pageIndex, int pageSize);

        IEnumerable<PrepaidBill> GetArrearsPrepaidBills(string buildingNo);

        int GetArrearsPrepaidBillsCount(string buildingNo);

        IEnumerable<PrepaidBill> GetArrearsPrepaidPagerBills(string buildingNo, int pageIndex, int pageSize);
    }
}