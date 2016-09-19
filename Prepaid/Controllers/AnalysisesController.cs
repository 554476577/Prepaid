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
        public IHttpActionResult GetBuildingStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var statises = this.deviceRepository.GetBuildingStatisInfo();
            var items = new
            {
                BuildingNos = from item in statises select item.xAxis,
                Values = from item in statises select Math.Round(item.yAxis ?? 0.00, 2)
            };

            return Ok(items);
        }

        [Route("api/typestatis")]
        [HttpGet]
        public IHttpActionResult GetTypeStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var statises = this.deviceRepository.GetTypeStatisInfo();
            var items = new
            {
                DeviceTypes = from item in statises select item.xAxis,
                Values = from item in statises select Math.Round(item.yAxis ?? 0.00, 2)
            };

            return Ok(items);
        }

        [Route("api/buildingtypestatis/{buildingNo}")]
        [HttpGet]
        public IHttpActionResult GetBuildingTypeStatis(string buildingNo)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var statises = this.deviceRepository.GetBuildingTypeStatisInfo(buildingNo);
            var items = new
            {
                DeviceTypes = from item in statises select item.xAxis,
                Values = from item in statises select Math.Round(item.yAxis ?? 0.00, 2)
            };

            return Ok(items);
        }

        [Route("api/billstatis")]
        [HttpGet]
        public IHttpActionResult GetBillStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            List<string> Timelines = new List<string> { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
            IEnumerable<string> BuildingNos = from item in this.buildingRepository.GetAll() select item.BuildingNo;
            List<double>[] Valuelines = new List<double>[BuildingNos.Count()];
            for (int i = 0; i < BuildingNos.Count(); i++)
            {
                string buildingNo = BuildingNos.ElementAt(i);
                var lines = from item in this.billRepository.GetAll()
                            where item.Device.Room.BuildingNo == buildingNo && item.DateTime.Value.Year == DateTime.Now.Year
                            group item by new
                            {
                                DateTime = item.Remark
                            } into g
                            orderby g.Key.DateTime
                            select g.Sum(p => (p.CurValue - p.PreValue));
                Valuelines[i] = new List<double>();
                Valuelines[i].AddRange(lines);
            }

            var items = new
            {
                Timelines = Timelines,
                BuildingNos = BuildingNos,
                Valuelines = Valuelines
            };

            return Ok(items);
        }

        [Route("api/monthstatis")]
        [HttpGet]
        public IHttpActionResult GetMonthStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            List<string> Timelines = new List<string> { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
            IEnumerable<string> BuildingNos = from item in this.buildingRepository.GetAll() select item.BuildingNo;
            List<double>[] Valuelines = new List<double>[BuildingNos.Count()];
            IEnumerable<VBuildEp> BuildEps = this.deviceRepository.GetMonthEp();
            for (int i = 0; i < BuildingNos.Count(); i++)
            {
                string buildingNo = BuildingNos.ElementAt(i);
                var lines = from item in BuildEps
                            where item.BuildingNo == buildingNo && item.DateTime.Substring(0, 4) == DateTime.Now.Year.ToString()
                            select item.Value;

                Valuelines[i] = new List<double>();
                Valuelines[i].AddRange(lines);
            }

            var items = new
            {
                Timelines = Timelines,
                BuildingNos = BuildingNos,
                Valuelines = Valuelines
            };

            return Ok(items);
        }

        [Route("api/buildmonthstatis/{buildingNo}")]
        [HttpGet]
        public IHttpActionResult GetBuildMonthStatis(string buildingNo)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            List<string> Timelines = new List<string> { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
            var statises = this.deviceRepository.GetBuildingMonthEp(buildingNo);
            var items = new
            {
                Timelines = Timelines,
                Values = from item in statises select Math.Round(item.yAxis ?? 0.00, 2)
            };

            return Ok(items);
        }

        [Route("api/realtimefundstatis")]
        [HttpGet]
        public IHttpActionResult GetRealtimeFundStatis()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            IEnumerable<string> BuildingNos = from item in this.buildingRepository.GetAll() select item.BuildingNo;
            var statises = this.deviceRepository.GetRealtimeFunds();
            var Balance = statises.Sum(p => p.totalBalance);
            double Expend = 0.00;
            foreach (var item in statises)
            {
                Expend += Math.Round(item.totalExpend ?? 0.00, 2);
            }
            var items = new
            {
                BuildingNos = BuildingNos,
                TotalBalances = from item in statises select TextHelper.ConvertMoney(item.totalBalance),
                TotalExpends = from item in statises select TextHelper.ConvertMoney((int)(item.totalExpend ?? 0)),
                Balance = TextHelper.ConvertMoney(Balance),
                Expend = TextHelper.ConvertMoney((int)Expend),
                Percent = string.Format("{0:P}", Expend / Balance)
            };

            return Ok(items);
        }

        [Route("api/buildrealtimefundstatis/{buildingNo}")]
        [HttpGet]
        public IHttpActionResult GetBuildRealtimeFundStatis(string buildingNo)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            var item = this.deviceRepository.GetBuildingRealtimeFunds(buildingNo);
            return Ok(item);
        }
    }
}