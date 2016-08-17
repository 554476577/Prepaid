using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public interface IRechargeRespository : IRepository<string, Recharge>
    {
        IEnumerable<Recharge> GetAll(string RoomNo, string RealName);

        int GetCount(string RoomNo, string RealName);

        IEnumerable<Recharge> GetPagerItems(string RoomNo, string RealName, int pageIndex, int pageSize, Func<Recharge, DateTime?> func, bool isDesc = false);
    }
}