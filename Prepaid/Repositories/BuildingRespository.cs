using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class BuildingRespository : AbstractRespository<string, Building>
    {
        public override System.Data.Entity.DbSet<Building> GetAll()
        {
            return db.Buildings;
        }

        public override bool IsExist(string uuid)
        {
            return db.Buildings.Count(e => e.BuildingNo == uuid) > 0;
        }
    }
}