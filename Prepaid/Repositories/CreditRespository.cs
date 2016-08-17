using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class CreditRespository : AbstractRespository<string, Credit>
    {
        public override System.Data.Entity.DbSet<Credit> GetAll()
        {
            return db.Credits;
        }

        public override bool IsExist(string uuid)
        {
            return db.Credits.Count(e => e.UUID == uuid) > 0;
        }
    }
}