using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class UserRespository : AbstractRespository<string, User>
    {
        public override System.Data.Entity.DbSet<User> GetAll()
        {
            return db.Users;
        }

        public override bool IsExist(string uuid)
        {
            return db.Users.Count(e => e.UUID == uuid) > 0;
        }
    }
}