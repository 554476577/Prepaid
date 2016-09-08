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
    public class MsgsController : ApiController
    {
        IRepository<int, Msg> repository;

        public MsgsController(IRepository<int, Msg> repository)
        {
            this.repository = repository;
        }

        // GET: api/msgs
        public IHttpActionResult GetMsgs()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Msg> Msgs;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Msgs = this.repository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount());
                Msgs = this.repository.GetPagerItems(pageIndex, pageSize, u => u.ID);
            }

            var items = from item in Msgs
                        select new
                        {
                            ID = item.ID,
                            RoomNo = item.RoomNo,
                            RealName=item.Room.RealName,
                            Phone=item.Room.Phone,
                            Content = item.Content,
                            PostType = item.PostType,
                            Status = item.Status,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/msgs/3
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> GetMsg(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Msg item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                RoomNo = item.RoomNo,
                Content = item.Content,
                PostType = item.PostType,
                Status = item.Status,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/msgs/3
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMsg(int uuid, [FromUri]Msg Msg)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Msg.ID)
                return BadRequest();

            try
            {
                Msg.CreateTime = DateTime.Now;
                await this.repository.PutAsync(Msg);
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

        // POST: api/msgs
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> PostMsg([FromUri]Msg Msg)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Msg.CreateTime = DateTime.Now;
                await this.repository.AddAsync(Msg);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(Msg.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/msgs/3
        public async Task<IHttpActionResult> DeleteMsg(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Msg Msg = await this.repository.GetByIdAsync(uuid);
            if (Msg == null)
                return NotFound();

            await this.repository.DeleteAsync(Msg);

            return Ok();
        }
    }
}