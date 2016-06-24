using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IAdminRepository : IRepository<string, Admin>
    {
        Admin FindByUserNameAndPassword(string userName, string password);
    }
}