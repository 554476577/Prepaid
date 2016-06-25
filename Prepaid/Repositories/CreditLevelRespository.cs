using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class CreditLevelRespository : AbstractRespository<string, CreditLevel>
    {
        public override System.Data.Entity.DbSet<CreditLevel> GetAll()
        {
            return db.CreditLevels;
        }

        public override bool IsExist(string uuid)
        {
            return db.CreditLevels.Count(e => e.UUID == uuid) > 0;
        }
    }
}