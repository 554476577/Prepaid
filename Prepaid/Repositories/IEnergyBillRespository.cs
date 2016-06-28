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
    }
}