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
    public class AlarmsController : ApiController
    {
        IRepository<int, Alarm> repository;

        public AlarmsController(IRepository<int, Alarm> repository)
        {
            this.repository = repository;
        }

        // GET: api/alarms
        public IHttpActionResult GetAlarms()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Alarm> Alarms;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Alarms = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Alarms = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in Alarms
                        select new
                        {
                            ID = item.ID,
                            DeviceNo = item.DeviceNo,
                            DeviceName = item.Device.DeviceName,
                            Content = item.Content,
                            Type = item.Type,
                            Level = item.Level,
                            MsgID = item.MsgID,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/alarms/3
        [ResponseType(typeof(Alarm))]
        public async Task<IHttpActionResult> GetAlarm(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Alarm item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                DeviceNo = item.DeviceNo,
                DeviceName = item.Device.DeviceName,
                Content = item.Content,
                Type = item.Type,
                Level = item.Level,
                MsgID = item.MsgID,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/alarms/3
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAlarm(int uuid, [FromUri]Alarm Alarm)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Alarm.ID)
                return BadRequest();

            try
            {
                Alarm.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Alarm);
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

        // POST: api/alarms
        [ResponseType(typeof(Alarm))]
        public async Task<IHttpActionResult> PostAlarm([FromUri]Alarm Alarm)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Alarm.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Alarm);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Alarm.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/alarms/3
        public async Task<IHttpActionResult> DeleteAlarm(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Alarm Alarm = await this.repository.GetByIdAsync(uuid);
            if (Alarm == null)
                return NotFound();

            await this.repository.DeleteAsync(Alarm);

            return Ok();
        }
    }
}