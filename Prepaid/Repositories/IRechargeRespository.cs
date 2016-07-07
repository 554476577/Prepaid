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
        IEnumerable<Recharge> GetAll(string userID, string realName);

        int GetCount(string userID, string realName);

        IEnumerable<Recharge> GetPagerItems(string userID, string realName, int pageIndex, int pageSize, Func<Recharge, string> func, bool isDesc = false);
    }
}