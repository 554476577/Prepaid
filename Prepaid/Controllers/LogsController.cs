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
    public class LogsController : ApiController
    {
        ILogRepository repository;

        public LogsController(ILogRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/logs
        public IHttpActionResult GetLogs()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Log> logs;
            string UserName = HttpContext.Current.Request.Params["UserName"];
            string Type = HttpContext.Current.Request.Params["Type"];
            string StartTime = HttpContext.Current.Request.Params["StartTime"];
            string EndTime = HttpContext.Current.Request.Params["EndTime"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                logs = this.repository.GetAll(UserName, Type, StartTime, EndTime);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount(UserName, Type, StartTime, EndTime));
                logs = this.repository.GetPagerItems(UserName, Type, StartTime, EndTime, pageIndex, pageSize, u => u.ID);
            }

            var items = from item in logs
                        select new
                        {
                            ID = item.ID,
                            UserName = item.Admin.UserName,
                            RealName = item.Admin.RealName,
                            Phone = item.Admin.Phone,
                            Type = item.Type,
                            ClientAddr = item.ClientAddr,
                            Content = item.Content,
                            DateTime = item.DateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/logs/1
        [ResponseType(typeof(Log))]
        public async Task<IHttpActionResult> GetLog(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Log item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                UserName = item.Admin.UserName,
                RealName = item.Admin.RealName,
                Phone = item.Admin.Phone,
                Type = item.Type,
                ClientAddr = item.ClientAddr,
                Content = item.Content,
                DateTime = item.DateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/logs/1
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLog(int uuid, [FromUri]Log log)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != log.ID)
                return BadRequest();

            try
            {
                log.DateTime = DateTime.Now;
                await this.repository.PutAsync(log);
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

        // POST: api/logs
        [ResponseType(typeof(Log))]
        public async Task<IHttpActionResult> PostLog([FromUri]Log log)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                log.DateTime = DateTime.Now;
                await this.repository.AddAsync(log);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(log.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/logs/1
        public async Task<IHttpActionResult> DeleteLog(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Log log = await this.repository.GetByIdAsync(uuid);
            if (log == null)
                return NotFound();

            await this.repository.DeleteAsync(log);

            return Ok();
        }
    }
}