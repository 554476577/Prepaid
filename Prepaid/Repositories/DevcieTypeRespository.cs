using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class DevcieTypeRespository : AbstractRespository<string, DeviceType>
    {
        public override System.Data.Entity.DbSet<DeviceType> GetAll()
        {
            return db.DeviceTypes;
        }

        public override bool IsExist(string uuid)
        {
            return db.DeviceTypes.Count(e => e.UUID == uuid) > 0;
        }
    }
}