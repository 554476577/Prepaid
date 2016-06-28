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
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<UserEnergy> userEnergies;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                userEnergies = this.repository.GetUserEnergies();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                userEnergies = this.repository.GetUserPagerEnergies(pageIndex, pageSize, u => u.UserID);
            }
            pager.Items = userEnergies;

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