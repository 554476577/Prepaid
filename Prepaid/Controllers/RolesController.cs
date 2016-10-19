using Prepaid.Models;
using Prepaid.Repositories;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Prepaid.Controllers
{
    public class RolesController : ApiController
    {
        IRoleRepository repository;

        public RolesController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/roles
        public IHttpActionResult GetRoles()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<Role> roles = this.repository.GetAll(1);
            var items = from item in roles
                        select new
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };

            return Ok(items);
        }

        // GET: api/roles/1
        public async Task<IHttpActionResult> GetRole(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Role item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/roles/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRole(int uuid, [FromUri]Role role)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != role.ID)
                return BadRequest();

            try
            {
                role.CreateTime = DateTime.Now;
                await this.repository.PutAsync(role);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.repository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/roles
        [ResponseType(typeof(Role))]
        public async Task<IHttpActionResult> PostRole([FromUri]Role role)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                role.CreateTime = DateTime.Now;
                await this.repository.AddAsync(role);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(role.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/roles/1
        public async Task<IHttpActionResult> DeleteRole(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Role role = await this.repository.GetByIdAsync(uuid);
            if (role == null)
                return NotFound();

            await this.repository.DeleteAsync(role);

            return Ok();
        }
    }
}