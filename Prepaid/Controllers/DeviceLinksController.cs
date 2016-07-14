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
    public class DeviceLinksController : ApiController
    {
        IRepository<int, DeviceLink> repository;

        public DeviceLinksController(IRepository<int, DeviceLink> repository)
        {
            this.repository = repository;
        }

        // GET: api/devicelinks
        public IHttpActionResult GetDeviceLinks()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<DeviceLink> deviceLinks;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                deviceLinks = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                deviceLinks = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in deviceLinks
                        select new
                        {
                            ID = item.ID,
                            UserID = item.UserID,
                            RealName = item.User.RealName,
                            PointID = item.PointID,
                            DeviceName = item.Point.DeviceName,
                            Status = item.Status,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/devicelinks/1
        [ResponseType(typeof(DeviceLink))]
        public async Task<IHttpActionResult> GetDeviceLink(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            DeviceLink item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                UserID = item.UserID,
                RealName = item.User.RealName,
                PointID = item.PointID,
                DeviceName = item.Point.DeviceName,
                Status = item.Status,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/devicelinks/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDeviceLink(int uuid, [FromUri]DeviceLink deviceLink)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != deviceLink.ID)
                return BadRequest();

            try
            {
                deviceLink.CreateTime = DateTime.Now;
                await this.repository.PutAsync(deviceLink);
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

        // POST: api/devicelinks
        [ResponseType(typeof(DeviceLink))]
        public async Task<IHttpActionResult> PostDeviceLink([FromUri]DeviceLink deviceLink)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                deviceLink.CreateTime = DateTime.Now;
                await this.repository.AddAsync(deviceLink);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(deviceLink.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/devicelinks/1
        public async Task<IHttpActionResult> DeleteDeviceLink(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            DeviceLink deviceLink = await this.repository.GetByIdAsync(uuid);
            if (deviceLink == null)
                return NotFound();

            await this.repository.DeleteAsync(deviceLink);

            return Ok();
        }
    }
}