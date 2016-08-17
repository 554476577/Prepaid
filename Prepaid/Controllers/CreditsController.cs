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
    public class CreditsController : ApiController
    {
        IRepository<string, Credit> repository;

        public CreditsController(IRepository<string, Credit> repository)
        {
            this.repository = repository;
        }

        // GET: api/credits
        public IHttpActionResult GetCredits()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Credit> Credits;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Credits = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Credits = this.repository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in Credits
                        select new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            MinScore = item.MinScore,
                            MaxScore = item.MaxScore,
                            Arrears = TextHelper.ConvertMoney(item.Arrears),
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/credits/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Credit))]
        public async Task<IHttpActionResult> GetCredit(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Credit item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                Name = item.Name,
                Description = item.Description,
                MinScore = item.MinScore,
                MaxScore = item.MaxScore,
                Arrears = TextHelper.ConvertMoney(item.Arrears),
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/credits/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCredit(string uuid, [FromUri]Credit Credit)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Credit.UUID)
                return BadRequest();

            try
            {
                Credit.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Credit);
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

        // POST: api/credits
        [ResponseType(typeof(Credit))]
        public async Task<IHttpActionResult> PostCredit([FromUri]Credit Credit)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Credit.UUID = TextHelper.GenerateUUID();
                Credit.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Credit);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Credit.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/credits/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteCredit(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Credit Credit = await this.repository.GetByIdAsync(uuid);
            if (Credit == null)
                return NotFound();

            await this.repository.DeleteAsync(Credit);

            return Ok();
        }
    }
}