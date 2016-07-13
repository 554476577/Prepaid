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
    public class CreditLevelsController : ApiController
    {
        IRepository<string, CreditLevel> repository;

        public CreditLevelsController(IRepository<string, CreditLevel> repository)
        {
            this.repository = repository;
        }

        // GET: api/creditlevels
        public IHttpActionResult GetCreditLevels()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<CreditLevel> creditLevels;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                creditLevels = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                creditLevels = this.repository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in creditLevels
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

        // GET: api/creditlevels/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(CreditLevel))]
        public async Task<IHttpActionResult> GetCreditLevel(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            CreditLevel item = await this.repository.GetByIdAsync(uuid);
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

        // PUT: api/creditlevels/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCreditLevel(string uuid, [FromUri]CreditLevel creditLevel)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != creditLevel.UUID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(creditLevel);
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

        // POST: api/creditlevels
        [ResponseType(typeof(CreditLevel))]
        public async Task<IHttpActionResult> PostCreditLevel([FromUri]CreditLevel creditLevel)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                creditLevel.UUID = TextHelper.GenerateUUID();
                creditLevel.CreateTime = DateTime.Now;
                await this.repository.AddAsync(creditLevel);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(creditLevel.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/creditlevels/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteCreditLevel(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            CreditLevel creditLevel = await this.repository.GetByIdAsync(uuid);
            if (creditLevel == null)
                return NotFound();

            await this.repository.DeleteAsync(creditLevel);

            return Ok();
        }
    }
}