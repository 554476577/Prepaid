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
    public class UsersController : ApiController
    {
        IUserRespository repository;

        public UsersController(IUserRespository repository)
        {
            this.repository = repository;
        }

        // GET: api/users
        public IHttpActionResult GetUsers()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<User> users;
            string UserID = HttpContext.Current.Request.Params["UserID"];
            string RealName = HttpContext.Current.Request.Params["RealName"];
            string BuildingName = HttpContext.Current.Request.Params["BuildingName"];
            string RoomNo = HttpContext.Current.Request.Params["RoomNo"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                users = this.repository.GetAll(UserID, RealName, BuildingName, RoomNo);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetCount(UserID, RealName, BuildingName, RoomNo));
                users = this.repository.GetPagerItems(UserID, RealName, BuildingName, RoomNo, pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in users
                        select new
                        {
                            UUID = item.UUID,
                            Phone = item.Phone,
                            RealName = item.RealName,
                            Address = item.Address,
                            BuildingNo = item.BuildingNo,
                            BuildingName = item.BuildingName,
                            FloorCount = item.FloorCount,
                            Floor = item.Floor,
                            RoomNo = item.RoomNo,
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

        // GET: api/users/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            User item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                Phone = item.Phone,
                RealName = item.RealName,
                Address = item.Address,
                BuildingNo = item.BuildingNo,
                BuildingName = item.BuildingName,
                FloorCount = item.FloorCount,
                Floor = item.Floor,
                RoomNo = item.RoomNo,
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

        // PUT: api/users/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string uuid, [FromUri]User user)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != user.UUID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(user);
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

        // POST: api/users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser([FromUri]User user)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                user.UUID = TextHelper.GenerateUUID();
                user.CreateTime = DateTime.Now;
                await this.repository.AddAsync(user);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(user.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/users/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteUser(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            User user = await this.repository.GetByIdAsync(uuid);
            if (user == null)
                return NotFound();

            await this.repository.DeleteAsync(user);

            return Ok();
        }
    }
}