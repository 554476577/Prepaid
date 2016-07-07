using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public interface IUserRespository : IRepository<string, User>
    {
        IEnumerable<User> GetAll(string userID, string realName, string buildingName, string roomNo);

        int GetCount(string userID, string realName, string buildingName, string roomNo);

        IEnumerable<User> GetPagerItems(string userID, string realName, string buildingName, string roomNo,
            int pageIndex, int pageSize, Func<User, string> func, bool isDesc = false);
    }
}