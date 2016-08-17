using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class CommunityRespository : AbstractRespository<string, Community>
    {
        public override System.Data.Entity.DbSet<Community> GetAll()
        {
            return db.Communities;
        }

        public override bool IsExist(string uuid)
        {
            return db.Communities.Count(e => e.UUID == uuid) > 0;
        }
    }
}