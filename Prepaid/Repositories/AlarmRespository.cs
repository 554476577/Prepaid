using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class AlarmRespository : AbstractRespository<int, Alarm>
    {
        public override System.Data.Entity.DbSet<Alarm> GetAll()
        {
            return db.Alarms;
        }

        public override bool IsExist(int uuid)
        {
            return db.Alarms.Count(e => e.ID == uuid) > 0;
        }
    }
}