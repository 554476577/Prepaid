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
    public class CommunitiesController : ApiController
    {
        IRepository<string, Community> repository;

        public CommunitiesController(IRepository<string, Community> repository)
        {
            this.repository = repository;
        }

        // GET: api/communities
        public IHttpActionResult GetCommunities()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Community> Communities;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Communities = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Communities = this.repository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in Communities
                        select new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Address = item.Address,
                            Area = item.Area,
                            Capacity = item.Capacity,
                            CheckInTime = item.CheckInTime,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/communities/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Community))]
        public async Task<IHttpActionResult> GetCommunity(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Community item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                Name = item.Name,
                Description = item.Description,
                Address = item.Address,
                Area = item.Area,
                Capacity = item.Capacity,
                CheckInTime = item.CheckInTime,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/communities/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCommunity(string uuid, [FromUri]Community Community)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Community.UUID)
                return BadRequest();

            try
            {
                Community.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Community);
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

        // POST: api/communities
        [ResponseType(typeof(Community))]
        public async Task<IHttpActionResult> PostCommunity([FromUri]Community Community)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Community.UUID = TextHelper.GenerateUUID();
                Community.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Community);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Community.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/communities/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteCommunity(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Community Community = await this.repository.GetByIdAsync(uuid);
            if (Community == null)
                return NotFound();

            await this.repository.DeleteAsync(Community);

            return Ok();
        }
    }
}