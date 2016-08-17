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
    public class DeviceTypesController : ApiController
    {
        IRepository<string, DeviceType> repository;

        public DeviceTypesController(IRepository<string, DeviceType> repository)
        {
            this.repository = repository;
        }

        // GET: api/devicetypes
        public IHttpActionResult GetDeviceTypes()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<DeviceType> DeviceTypes;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                DeviceTypes = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                DeviceTypes = this.repository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in DeviceTypes
                        select new
                        {
                            UUID = item.UUID,
                            Name = item.Name,
                            Description = item.Description,
                            Price1 = item.Price1,
                            Price2 = item.Price2,
                            Price3 = item.Price3,
                            Price4 = item.Price4,
                            Price5 = item.Price5,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/devicetypes/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(DeviceType))]
        public async Task<IHttpActionResult> GetDeviceType(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            DeviceType item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                Name = item.Name,
                Description = item.Description,
                Price1 = item.Price1,
                Price2 = item.Price2,
                Price3 = item.Price3,
                Price4 = item.Price4,
                Price5 = item.Price5,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/devicetypes/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDeviceType(string uuid, [FromUri]DeviceType DeviceType)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != DeviceType.UUID)
                return BadRequest();

            try
            {
                DeviceType.CreateTime = DateTime.Now;
                await this.repository.PutAsync(DeviceType);
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

        // POST: api/devicetypes
        [ResponseType(typeof(DeviceType))]
        public async Task<IHttpActionResult> PostDeviceType([FromUri]DeviceType DeviceType)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                DeviceType.UUID = TextHelper.GenerateUUID();
                DeviceType.CreateTime = DateTime.Now;
                await this.repository.AddAsync(DeviceType);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(DeviceType.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/devicetypes/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteDeviceType(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            DeviceType DeviceType = await this.repository.GetByIdAsync(uuid);
            if (DeviceType == null)
                return NotFound();

            await this.repository.DeleteAsync(DeviceType);

            return Ok();
        }
    }
}