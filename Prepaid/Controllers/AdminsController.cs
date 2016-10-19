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
    public class AdminsController : ApiController
    {
        IAdminRepository adminRepository;
        ILogRepository logRepository;

        public AdminsController(IAdminRepository adminRepository, ILogRepository logRepository)
        {
            this.adminRepository = adminRepository;
            this.logRepository = logRepository;
        }

        // GET: api/admins
        public IHttpActionResult GetAdmins()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Admin> admins;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                admins = this.adminRepository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.adminRepository.GetCount());
                admins = this.adminRepository.GetPagerItems(pageIndex, pageSize, u => u.UUID);
            }

            var items = from item in admins
                        select new
                        {
                            UUID = item.UUID,
                            RoleID = item.RoleID,
                            RoleName = item.Role.Name,
                            UserName = item.UserName,
                            Password = item.Password,
                            RealName = item.RealName,
                            Phone = item.Phone,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/admins/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Admin))]
        public async Task<IHttpActionResult> GetAdmin(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Admin item = await this.adminRepository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                UUID = item.UUID,
                RoleID = item.RoleID,
                RoleName = item.Role.Name,
                UserName = item.UserName,
                Password = item.Password,
                RealName = item.RealName,
                Phone = item.Phone,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/admins/13cbc2e968bb42a5a60a59742c8684cc
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdmin(string uuid, [FromUri]Admin admin)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != admin.UUID)
                return BadRequest();

            try
            {
                admin.Password = TextHelper.MD5Encrypt(admin.Password);
                admin.CreateTime = DateTime.Now;
                await this.adminRepository.PutAsync(admin);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.adminRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/admins
        [ResponseType(typeof(Admin))]
        public async Task<IHttpActionResult> PostAdmin([FromUri]Admin admin)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                admin.UUID = Prepaid.Utils.TextHelper.GenerateUUID();
                admin.Password = TextHelper.MD5Encrypt(admin.Password);
                admin.CreateTime = DateTime.Now;
                await this.adminRepository.AddAsync(admin);
            }
            catch (DbUpdateException)
            {
                if (this.adminRepository.IsExist(admin.UUID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/admins/13cbc2e968bb42a5a60a59742c8684cc
        public async Task<IHttpActionResult> DeleteAdmin(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Admin admin = await this.adminRepository.GetByIdAsync(uuid);
            if (admin == null)
                return NotFound();

            await this.adminRepository.DeleteAsync(admin);

            return Ok();
        }

        [Route("api/admin")]
        [HttpGet]
        [ResponseType(typeof(AdminSession))]
        public IHttpActionResult GetAdminSession()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            AdminSession session = HttpContext.Current.Session["mySession"] as AdminSession;
            return Ok(session);
        }

        [Route("api/admins/login")]
        [HttpGet]
        public IHttpActionResult Login(string userName, string password)
        {
            Admin admin = this.adminRepository.FindByUserNameAndPassword(userName, password);
            if (admin == null)
                return NotFound();

            Log log = new Log();
            log.UserID = admin.UUID;
            log.Type = 1; // 1:登录日志 2:操作日志
            log.ClientAddr = TextHelper.GetHostAddress();
            log.Content = string.Format("管理员:{0}登录成功!", admin.UserName);
            log.DateTime = DateTime.Now;
            log.Remark = "";
            this.logRepository.Add(log);

            AdminSession session = new AdminSession();
            session.UUID = admin.UUID;
            session.UserName = admin.UserName;
            session.RealName = admin.RealName;
            session.Phone = admin.Phone;
            HttpContext.Current.Session["mySession"] = session;

            return Ok();
        }

        [Route("api/admins/logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.Session["mySession"] = null;
            return Ok();
        }
    }
}