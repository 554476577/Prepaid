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
        IRepository<string, Building> buildingRepository;
        IDeviceRepository deviceRepository;
        IBillRespository billRepository;

        public AnalysisesController(IRepository<string, Building> buildingRepository,
            IDeviceRepository deviceRepository, IBillRespository billRepository)
        {
            this.buildingRepository = buildingRepository;
            this.deviceRepository = deviceRepository;
            this.billRepository = billRepository;
        }

        [Route("api/buildingstatis")]
        [HttpGet]
        [ResponseType(typeof(Statis))]
        public IHttpActionResult GetBuildingStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            return Ok(this.deviceRepository.GetBuildingStatisInfo());
        }

        [Route("api/typestatis")]
        [HttpGet]
        public IHttpActionResult GetTypeStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            return Ok(this.deviceRepository.GetTypeStatisInfo());
        }

        [Route("api/billstatis")]
        [HttpGet]
        public IHttpActionResult GetBillStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            List<string> Timelines = { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
            IEnumerable<string> BuildingNos = from item in this.buildingRepository.GetAll() select item.BuildingNo;
            IEnumerable<double>[] Valuelines = new List<double>[BuildingNos.Count()];
            for (int i = 0; i < BuildingNos.Count(); i++)
            {
                string yearMonth = string.Format("{0}-{1}", DateTime.Now.Year, (i + 1).ToString("00"));
                var lines = from item in this.billRepository.GetAll()
                            where item.Remark == yearMonth
                            group item by new
                            {
                                BuildingNo = item.Device.Room.BuildingNo
                            } into g
                            orderby g.Key.BuildingNo
                            select new { Value = g.Sum(p => (p.CurValue - p.PreValue)) };
                Valuelines[i] = lines as IEnumerable<double>;
            }

            var items = new
            {
                Timelines = Timelines,
                BuildingNos = BuildingNos,
                Valuelines = Valuelines
            };

            return Ok(items);
        }
    }
}