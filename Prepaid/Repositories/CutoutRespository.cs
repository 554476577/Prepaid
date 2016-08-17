using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class CutoutRespository : AbstractRespository<int, Cutout>
    {
        public override System.Data.Entity.DbSet<Cutout> GetAll()
        {
            return db.Cutouts;
        }

        public override bool IsExist(int uuid)
        {
            return db.Cutouts.Count(e => e.ID == uuid) > 0;
        }
    }
}