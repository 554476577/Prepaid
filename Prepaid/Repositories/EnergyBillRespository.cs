using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class EnergyBillRespository : AbstractRespository<int, EnergyBill>
    {
        public override System.Data.Entity.DbSet<EnergyBill> GetAll()
        {
            return db.EnergyBills;
        }

        public override bool IsExist(int uuid)
        {
            return db.EnergyBills.Count(e => e.ID == uuid) > 0;
        }
    }
}