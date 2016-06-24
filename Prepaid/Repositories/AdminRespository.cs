using Prepaid.Models;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Repositories
{
    public class AdminRespository : AbstractRespository<string, Admin>, IAdminRepository
    {
        public override System.Data.Entity.DbSet<Admin> GetAll()
        {
            return db.Admins;
        }

        public override bool IsExist(string uuid)
        {
            return db.Admins.Count(e => e.UUID == uuid) > 0;
        }

        public Admin FindByUserNameAndPassword(string userName, string password)
        {
            string encryptPwd = TextHelper.MD5Encrypt(password);
            Admin admin = this.db.Admins.FirstOrDefault((u) => u.UserName == userName && u.Password == encryptPwd);
            return admin;
        }
    }
}