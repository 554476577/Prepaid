using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class DeviceLinkRespository : AbstractRespository<int, DeviceLink>
    {
        public override System.Data.Entity.DbSet<DeviceLink> GetAll()
        {
            return db.DeviceLinks;
        }

        public override bool IsExist(int uuid)
        {
            return db.DeviceLinks.Count(e => e.ID == uuid) > 0;
        }
    }
}