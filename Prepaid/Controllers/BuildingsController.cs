﻿using Newtonsoft.Json;
using Prepaid.Models;
using Prepaid.Repositories;
using Prepaid.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Prepaid.Controllers
{
    public class BuildingsController : ApiController
    {
        IRepository<string, Building> buildingRepository;
        IRoomRespository roomRepository;

        public BuildingsController(IRepository<string, Building> buildingRepository, IRoomRespository roomRepository)
        {
            this.buildingRepository = buildingRepository;
            this.roomRepository = roomRepository;
        }

        // GET: api/buildings
        public IHttpActionResult GetBuildings()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];
            IEnumerable<Building> Buildings;

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                Buildings = this.buildingRepository.GetAll();
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.buildingRepository.GetCount());
                Buildings = this.buildingRepository.GetPagerItems(pageIndex, pageSize, u => u.BuildingNo);
            }

            var items = from item in Buildings
                        select new
                        {
                            BuildingNo = item.BuildingNo,
                            Name = item.Name,
                            CommunityID = item.CommunityID,
                            CommunityName = item.Community.Name,
                            Description = item.Description,
                            Floors = item.Floors,
                            CreateTime = item.CreateTime,
                            Remark = item.Remark
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/cachebuildings
        [HttpGet]
        [Route("api/cachebuildings")]
        public IHttpActionResult GetCacheBuildings()
        {
            int isTimingSettle = Convert.ToInt32(WebConfigHelper.ReadAppSetting("IsTimingSettle"));
            var buildings = TextHelper.GetCacheBuildings(this.buildingRepository, this.roomRepository);
            var items = new
            {
                IsTimingSettle = isTimingSettle,
                Buildings = buildings
            };

            return Ok(items);
        }

        // GET: api/buildings/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(Building))]
        public async Task<IHttpActionResult> GetBuilding(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Building item = await this.buildingRepository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                BuildingNo = item.BuildingNo,
                Name = item.Name,
                CommunityID = item.CommunityID,
                CommunityName = item.Community.Name,
                Description = item.Description,
                Floors = item.Floors,
                CreateTime = item.CreateTime,
                Remark = item.Remark
            };

            return Ok(result);
        }

        // PUT: api/buildings/03b96c82ba5747eba2a5d96ef67837c9
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBuilding(string uuid, [FromUri]Building Building)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != Building.BuildingNo)
                return BadRequest();

            try
            {
                Building.CreateTime = DateTime.Now;
                await this.buildingRepository.PutAsync(Building);
                TextHelper.SetCacheBuilding(this.buildingRepository, this.roomRepository);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.buildingRepository.IsExist(uuid))
                    return NotFound();
                else
                    throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/buildings
        [ResponseType(typeof(Building))]
        public async Task<IHttpActionResult> PostBuilding([FromUri]Building Building)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                Building.CreateTime = DateTime.Now;
                await this.buildingRepository.AddAsync(Building);
                TextHelper.SetCacheBuilding(this.buildingRepository, this.roomRepository);
            }
            catch (DbUpdateException)
            {
                if (this.buildingRepository.IsExist(Building.BuildingNo))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/buildings/03b96c82ba5747eba2a5d96ef67837c9
        public async Task<IHttpActionResult> DeleteBuilding(string uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Building Building = await this.buildingRepository.GetByIdAsync(uuid);
            if (Building == null)
                return NotFound();

            await this.buildingRepository.DeleteAsync(Building);
            TextHelper.SetCacheBuilding(this.buildingRepository, this.roomRepository);

            return Ok();
        }
    }
}