using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class RechargeRespository : AbstractRespository<string, Recharge>
    {
        public override System.Data.Entity.DbSet<Recharge> GetAll()
        {
            return db.Recharges;
        }

        public override bool IsExist(string uuid)
        {
            return db.Recharges.Count(e => e.UUID == uuid) > 0;
        }
    }
}