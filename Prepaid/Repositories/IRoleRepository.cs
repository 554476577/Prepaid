using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prepaid.Repositories
{
    public interface IRoleRepository : IRepository<int, Role>
    {
        IEnumerable<Role> GetAll(int status);
    }
}