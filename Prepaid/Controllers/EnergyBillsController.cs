using Newtonsoft.Json;
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
    public class EnergyBillsController : ApiController
    {
        IEnergyBillRespository repository;

        public EnergyBillsController(IEnergyBillRespository repository)
        {
            this.repository = repository;
        }

        // GET: api/energybills
        public IHttpActionResult GetEnergyBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<EnergyBill> energyBills;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                energyBills = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                energyBills = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in energyBills
                        select new
                        {
                            ID = item.ID,
                            DeviceLinkID = item.DeviceLinkID,
                            UserID = item.DeviceLink.User.UUID,
                            RealName = item.DeviceLink.User.RealName,
                            PointID = item.DeviceLink.Point.ID,
                            DeviceName = item.DeviceLink.Point.DeviceName,
                            TotolValue = item.TotolValue,
                            Value = item.Value,
                            Money = item.Money,
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/userenergybills
        [HttpGet]
        [Route("api/userenergybills")]
        public IHttpActionResult GetUserEnergyBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<UserEnergy> userEnergies;
            string UserID = HttpContext.Current.Request.Params["UserID"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string StartTime = HttpContext.Current.Request.Params["StartTime"];
            string EndTime = HttpContext.Current.Request.Params["EndTime"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                userEnergies = this.repository.GetUserEnergies(UserID, RealName, StartTime, EndTime);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetUserEnergiesCount(UserID, RealName, StartTime, EndTime));
                userEnergies = this.repository.GetUserPagerEnergies(UserID, RealName, StartTime, EndTime, pageIndex, pageSize, u => u.UserID);
            }
            pager.Items = userEnergies;

            return Ok(pager);
        }

        [HttpGet]
        [Route("api/export/userenergybills")]
        public IHttpActionResult ExportUserEnergyBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<UserEnergy> userEnergies = this.repository.GetUserEnergies();
            string path = ReportHelper.ExportUserEnergies(userEnergies);
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.Write(path);

            return Ok();
        }

        // GET: api/userprepaidbills
        [HttpGet]
        [Route("api/userprepaidbills")]
        public IHttpActionResult GetUserPrepaidBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<PrepaidEnergy> prepaidEnergies;
            string UserID = HttpContext.Current.Request.Params["UserID"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string BuildingName = HttpContext.Current.Request.Params["BuildingName"];
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                prepaidEnergies = this.repository.GetPrepaidEnergies(UserID, RealName, BuildingName, RoomNo);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetPrepaidEnergiesCount(UserID, RealName, BuildingName, RoomNo));
                prepaidEnergies = this.repository.GetPrepaidPagerEnergies(UserID, RealName, BuildingName, RoomNo, pageIndex, pageSize, u => u.UserID);
            }
            pager.Items = prepaidEnergies;

            return Ok(pager);
        }

        // GET: api/energybills/1
        [ResponseType(typeof(EnergyBill))]
        public async Task<IHttpActionResult> GetEnergyBill(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            EnergyBill item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                DeviceLinkID = item.DeviceLinkID,
                UserID = item.DeviceLink.User.UUID,
                RealName = item.DeviceLink.User.RealName,
                PointID = item.DeviceLink.Point.ID,
                DeviceName = item.DeviceLink.Point.DeviceName,
                TotolValue = item.TotolValue,
                Value = item.Value,
                Money = item.Money,
                DateTime = item.DateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/energybills/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEnergyBill(int uuid, [FromUri]EnergyBill energyBill)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != energyBill.ID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(energyBill);
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

        // POST: api/energybills
        [ResponseType(typeof(EnergyBill))]
        public async Task<IHttpActionResult> PostEnergyBill([FromUri]EnergyBill energyBill)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                energyBill.DateTime = DateTime.Now;
                await this.repository.AddAsync(energyBill);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(energyBill.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // POST: api/userenergybills
        [HttpPost]
        [Route("api/userenergybills")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostUserEnergyBills()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            string strDeviceEnergies = HttpContext.Current.Request.Params["DeviceEnergies"];
            strDeviceEnergies = string.Format("[{0}]", strDeviceEnergies); // 格式化为json数组
            List<InstantDeviceEnergy> deviceEnergies = JsonConvert.DeserializeObject<List<InstantDeviceEnergy>>(strDeviceEnergies);
            DateTime now = DateTime.Now;
            foreach (InstantDeviceEnergy item in deviceEnergies)
            {
                EnergyBill energyBill = new EnergyBill();
                energyBill.DeviceLinkID = item.DeviceLinkID;
                energyBill.TotolValue = item.CurrentValue;
                energyBill.Value = item.IntervalValue;
                energyBill.Money = item.IntervalMoney;
                energyBill.DateTime = now;
                await this.repository.AddAsync(energyBill);
            }

            return Ok();
        }

        // DELETE: api/energybills/1
        public async Task<IHttpActionResult> DeleteEnergyBill(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            EnergyBill energyBill = await this.repository.GetByIdAsync(uuid);
            if (energyBill == null)
                return NotFound();

            await this.repository.DeleteAsync(energyBill);

            return Ok();
        }
    }
}