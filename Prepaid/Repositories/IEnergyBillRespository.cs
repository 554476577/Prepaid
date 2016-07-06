using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IEnergyBillRespository : IRepository<int, EnergyBill>
    {
        IEnumerable<UserEnergy> GetUserEnergies();

        IEnumerable<UserEnergy> GetUserPagerEnergies(int pageIndex, int pageSize, Func<UserEnergy, string> func, bool isDesc = false);

        IEnumerable<UserEnergy> GetUserEnergies(string userID, string realName, string startTime, string endTime);

        IEnumerable<UserEnergy> GetUserPagerEnergies(string userID, string realName, string startTime, string endTime,
            int pageIndex, int pageSize, Func<UserEnergy, string> func, bool isDesc = false);

        IEnumerable<PrepaidEnergy> GetPrepaidEnergies();

        IEnumerable<PrepaidEnergy> GetPrepaidPagerEnergies(int pageIndex, int pageSize, Func<PrepaidEnergy, string> func, bool isDesc = false);

        IEnumerable<PrepaidEnergy> GetPrepaidEnergies(string userID, string realName, string buildingName, string roomNo);

        IEnumerable<PrepaidEnergy> GetPrepaidPagerEnergies(string userID, string realName, string buildingName, string roomNo,
            int pageIndex, int pageSize, Func<PrepaidEnergy, string> func, bool isDesc = false);
    }
}