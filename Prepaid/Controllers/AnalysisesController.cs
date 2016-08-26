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
    public class AnalysisesController : ApiController
    {
        IDeviceRepository deviceRepository;

        public AnalysisesController(IDeviceRepository deviceRepository)
        {
            this.deviceRepository = deviceRepository;
        }

        [Route("api/buildingstatis")]
        [HttpGet]
        [ResponseType(typeof(BuildingStatis))]
        public IHttpActionResult GetBuildingStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            BuildingStatis statis = this.deviceRepository.GetBuildingStatisInfo();

            return Ok(statis);
        }
    }
}