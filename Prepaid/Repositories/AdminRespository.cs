using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class AdminRespository : AbstractRespository<string, Admin>
    {
        public override System.Data.Entity.DbSet<Admin> GetAll()
        {
            return db.Admins;
        }

        public override bool IsExist(string uuid)
        {
            return db.Admins.Count(e => e.UUID == uuid) > 0;
        }
    }
}