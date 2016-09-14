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
    public class LaddersController : ApiController
    {
        IRepository<string, Ladder> repository;

        public LaddersController(IRepository<string, Ladder> repository)
        {
            this.repository = repository;
        }

        // GET: api/ladders
        public IHttpActionResult GetLadders()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Ladder> Ladders;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Ladders = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Ladders = this.repository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in Ladders
                        select new
                        {
                            UUID = item.UUID,
                            TypeID = item.TypeID,
                            TypeName = item.DeviceType.Name,
                            Name = item.Name,
                            Description = item.Description,
                            MinEnergy = item.MinEnergy,
                            MaxEnergy = item.MaxEnergy,
                            Price = TextHelper.ConvertMoney(item.Price),
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/ladders/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Ladder))]
        public async Task<IHttpActionResult> GetLadder(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Ladder item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                TypeID = item.TypeID,
                TypeName = item.DeviceType.Name,
                Name = item.Name,
                Description = item.Description,
                MinEnergy = item.MinEnergy,
                MaxEnergy = item.MaxEnergy,
                Price = TextHelper.ConvertMoney(item.Price),
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/ladders/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLadder(string uuid, [FromUri]Ladder Ladder)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Ladder.UUID)
                return BadRequest();

            try
            {
                Ladder.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Ladder);
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

        // POST: api/ladders
        [ResponseType(typeof(Ladder))]
        public async Task<IHttpActionResult> PostLadder([FromUri]Ladder Ladder)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Ladder.UUID = TextHelper.GenerateUUID();
                Ladder.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Ladder);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Ladder.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/ladders/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteLadder(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Ladder Ladder = await this.repository.GetByIdAsync(uuid);
            if (Ladder == null)
                return NotFound();

            await this.repository.DeleteAsync(Ladder);

            return Ok();
        }
    }
}