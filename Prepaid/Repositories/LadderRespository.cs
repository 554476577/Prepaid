using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class LadderRespository : AbstractRespository<string, Ladder>
    {
        public override System.Data.Entity.DbSet<Ladder> GetAll()
        {
            return db.Ladders;
        }

        public override bool IsExist(string uuid)
        {
            return db.Ladders.Count(e => e.UUID == uuid) > 0;
        }
    }
}