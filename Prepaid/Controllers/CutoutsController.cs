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
    public class CutoutsController : ApiController
    {
        IRepository<int, Cutout> repository;

        public CutoutsController(IRepository<int, Cutout> repository)
        {
            this.repository = repository;
        }

        // GET: api/cutouts
        public IHttpActionResult GetCutouts()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Cutout> Cutouts;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Cutouts = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Cutouts = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in Cutouts
                        select new
                        {
                            ID = item.ID,
                            DeviceNo = item.DeviceNo,
                            DeviceName = item.Device.DeviceName,
                            Reason = item.Reason,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/cutouts/3
        [ResponseType(typeof(Cutout))]
        public async Task<IHttpActionResult> GetCutout(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Cutout item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                DeviceNo = item.DeviceNo,
                DeviceName = item.Device.DeviceName,
                Reason = item.Reason,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/cutouts/3
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCutout(int uuid, [FromUri]Cutout Cutout)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Cutout.ID)
                return BadRequest();

            try
            {
                Cutout.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Cutout);
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

        // POST: api/cutouts
        [ResponseType(typeof(Cutout))]
        public async Task<IHttpActionResult> PostCutout([FromUri]Cutout Cutout)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Cutout.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Cutout);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Cutout.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/cutouts/3
        public async Task<IHttpActionResult> DeleteCutout(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Cutout Cutout = await this.repository.GetByIdAsync(uuid);
            if (Cutout == null)
                return NotFound();

            await this.repository.DeleteAsync(Cutout);

            return Ok();
        }
    }
}