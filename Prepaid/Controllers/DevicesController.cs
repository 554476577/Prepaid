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
    public class DevicesController : ApiController
    {
        IDeviceRepository repository;

        public DevicesController(IDeviceRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/devices
        public IHttpActionResult GetDevices()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Device> devices;
            string DeviceNo = HttpContext.Current.Request.Params["DeviceNo"];
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string ItemID = HttpContext.Current.Request.Params["ItemID"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                devices = this.repository.GetAll(DeviceNo, RoomNo, ItemID);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount(DeviceNo, RoomNo, ItemID));
                devices = this.repository.GetPagerItems(DeviceNo, RoomNo, ItemID, pageIndex, pageSize, u => u.DeviceNo);
            }

            var items = from item in devices
                        select new
                        {
                            DeviceNo = item.DeviceNo,
                            RoomNo = item.RoomNo,
                            TypeID = item.TypeID,
                            TypeName = item.DeviceType.Name,
                            Protocol = item.Protocol,
                            Scope = item.Scope,
                            DeviceName = item.DeviceName,
                            PhyAddr = item.PhyAddr,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            Status = item.Status,
                            Value = item.Value,
                            Rate = item.Rate,
                            Unit = item.DeviceType.Unit,
                            Price1 = TextHelper.ConvertMoney(item.DeviceType.Price1),
                            Price2 = TextHelper.ConvertMoney(item.DeviceType.Price2),
                            Price3 = TextHelper.ConvertMoney(item.DeviceType.Price3),
                            Price4 = TextHelper.ConvertMoney(item.DeviceType.Price4),
                            Price5 = TextHelper.ConvertMoney(item.DeviceType.Price5),
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            Remark1 = item.Remark1,
                            Remark2 = item.Remark2,
                            Remark3 = item.Remark3
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/devices/1
        [ResponseType(typeof(Device))]
        public async Task<IHttpActionResult> GetDevice(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Device item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                DeviceNo = item.DeviceNo,
                BuildingNo=item.Room.BuildingNo,
                BuildingName=item.Room.Building.Name,
                Floor=item.Room.Floor,
                RoomNo = item.RoomNo,
                TypeID = item.TypeID,
                TypeName = item.DeviceType.Name,
                Protocol = item.Protocol,
                Scope = item.Scope,
                DeviceName = item.DeviceName,
                PhyAddr = item.PhyAddr,
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                Status = item.Status,
                Value = item.Value,
                Rate = item.Rate,
                Unit = item.DeviceType.Unit,
                Price1 = TextHelper.ConvertMoney(item.DeviceType.Price1),
                Price2 = TextHelper.ConvertMoney(item.DeviceType.Price2),
                Price3 = TextHelper.ConvertMoney(item.DeviceType.Price3),
                Price4 = TextHelper.ConvertMoney(item.DeviceType.Price4),
                Price5 = TextHelper.ConvertMoney(item.DeviceType.Price5),
                IsArchive = item.IsArchive,
                ArchiveInterval = item.ArchiveInterval,
                Remark1 = item.Remark1,
                Remark2 = item.Remark2,
                Remark3 = item.Remark3
            };

            return Ok(result);
        }

        // PUT: api/devices/400001
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDevice(string uuid, [FromUri]Device device)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != device.DeviceNo)
                return BadRequest();

            try
            {
                Device item = this.repository.GetByID(uuid);
                item.DeviceNo = device.DeviceNo;
                item.RoomNo = device.RoomNo;
                item.TypeID = device.TypeID;
                item.Protocol = device.Protocol;
                item.Scope = device.Scope;
                item.DeviceName = device.DeviceName;
                item.PhyAddr = device.PhyAddr;
                item.ItemID = device.ItemID;
                item.ItemName = device.ItemName;
                item.ItemDescription = device.ItemDescription;
                item.Rate = device.Rate;
                item.IsArchive = device.IsArchive;
                item.ArchiveInterval = device.ArchiveInterval;
                await this.repository.PutAsync(item);
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

        // POST: api/devices
        [ResponseType(typeof(Device))]
        public async Task<IHttpActionResult> PostDevice([FromUri]Device device)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                device.Status = 0;
                device.DateTime = DateTime.Now;
                device.ArchiveTime = DateTime.Now;
                await this.repository.AddAsync(device);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(device.DeviceNo))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/devices/400001
        public async Task<IHttpActionResult> DeleteDevice(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Device device = await this.repository.GetByIdAsync(uuid);
            if (device == null)
                return NotFound();

            await this.repository.DeleteAsync(device);

            return Ok();
        }

        [Route("api/devices/upload")]
        [HttpPost]
        public async Task<IHttpActionResult> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return NotFound();

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                bool isDeleteAll = false;
                string[] values = provider.FormData.GetValues("IsDelete");
                if (values != null && values.Length > 0)
                    isDeleteAll = values[0] == "on" ? true : false;

                MultipartFileData file = provider.FileData[0];
                string fullName = file.LocalFileName;
                int rowAffected = await this.repository.BatchImport(file.LocalFileName, isDeleteAll);

                return Ok(rowAffected);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
    }
}