using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class MsgRespository : AbstractRespository<int, Msg>
    {
        public override System.Data.Entity.DbSet<Msg> GetAll()
        {
            return db.Msgs;
        }

        public override bool IsExist(int uuid)
        {
            return db.Msgs.Count(e => e.ID == uuid) > 0;
        }
    }
}