using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class RoleRespository : AbstractRespository<int, Role>, IRoleRepository
    {
        public override System.Data.Entity.DbSet<Role> GetAll()
        {
            return db.Roles;
        }

        public override bool IsExist(int uuid)
        {
            return db.Roles.Count(e => e.ID == uuid) > 0;
        }

        public new IEnumerable<Role> GetAll(int status)
        {
            return from item in GetAll() where item.Status == status select item;
        }
    }
}