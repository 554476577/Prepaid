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
    public class RoomsController : ApiController
    {
        IRoomRespository repository;

        public RoomsController(IRoomRespository repository)
        {
            this.repository = repository;
        }

        // GET: api/rooms
        public IHttpActionResult GetRooms()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Room> rooms;
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string BuildingNo = HttpContext.Current.Request.Params["BuildingNo"];
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string Floor = HttpContext.Current.Request.Params["Floor"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                rooms = this.repository.GetAll(RealName, BuildingNo, RoomNo, Floor);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount(RealName, BuildingNo, RoomNo, Floor));
                rooms = this.repository.GetPagerItems(RealName, BuildingNo, RoomNo, Floor, pageIndex, pageSize, u => u.RoomNo);
            }

            var items = from item in rooms
                        select new
                        {
                            RoomNo = item.RoomNo,
                            BuildingNo = item.BuildingNo,
                            BuildingName = item.Building.Name,
                            Floors = item.Building.Floors,
                            Floor = item.Floor,
                            Area = item.Area,
                            Price = item.Price,
                            RealName = item.RealName,
                            Phone = item.Phone,
                            AccountBalance = TextHelper.ConvertMoney(item.AccountBalance),
                            AccountWarnLimit = TextHelper.ConvertMoney(item.AccountWarnLimit),
                            CreditScore = item.CreditScore,
                            AlipayAccount = item.AlipayAccount,
                            WechatAccount = item.WechatAccount,
                            BankAccount = item.BankAccount,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/rooms/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> GetRoom(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Room item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                RoomNo = item.RoomNo,
                BuildingNo = item.BuildingNo,
                BuildingName = item.Building.Name,
                Floors = item.Building.Floors,
                Floor = item.Floor,
                Area = item.Area,
                Price = item.Price,
                RealName = item.RealName,
                Phone = item.Phone,
                AccountBalance = TextHelper.ConvertMoney(item.AccountBalance),
                AccountWarnLimit = TextHelper.ConvertMoney(item.AccountWarnLimit),
                CreditScore = item.CreditScore,
                AlipayAccount = item.AlipayAccount,
                WechatAccount = item.WechatAccount,
                BankAccount = item.BankAccount,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/rooms/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoom(string uuid, [FromUri]Room room)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != room.RoomNo)
                return BadRequest();

            try
            {
                room.CreateTime = DateTime.Now;
                await this.repository.PutAsync(room);
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

        // POST: api/rooms
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> PostRoom([FromUri]Room room)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                room.CreateTime = DateTime.Now;
                await this.repository.AddAsync(room);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(room.RoomNo))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/rooms/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteRoom(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Room room = await this.repository.GetByIdAsync(uuid);
            if (room == null)
                return NotFound();

            await this.repository.DeleteAsync(room);

            return Ok();
        }
    }
}