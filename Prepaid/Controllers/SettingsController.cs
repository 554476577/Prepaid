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
    public class SettingsController : ApiController
    {
        [Route("api/setting")]
        [HttpGet]
        [ResponseType(typeof(Setting))]
        public IHttpActionResult GetSystemSetting()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;
            Setting setting = TextHelper.GetSystemConfig();

            return Ok(setting);
        }

        [Route("api/setting")]
        [HttpPost]
        public IHttpActionResult PostSystemSetting([FromUri]Setting setting)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;
            TextHelper.SetSystemConfig(setting);

            return Ok();
        }
    }
}