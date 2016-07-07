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
    public class PointsController : ApiController
    {
        IPointRepository repository;

        public PointsController(IPointRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/points
        public IHttpActionResult GetPoints()
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Pager pager = null;
            IEnumerable<Point> points;
            string PointID = HttpContext.Current.Request.Params["PointID"];
            string DeviceName = HttpContext.Current.Request.Params["DeviceName"];
            string ItemID = HttpContext.Current.Request.Params["ItemID"];
            string strPageIndex = HttpContext.Current.Request.Params["PageIndex"];
            string strPageSize = HttpContext.Current.Request.Params["PageSize"];

            if (strPageIndex == null || strPageSize == null)
            {
                pager = new Pager();
                points = this.repository.GetOriginalAll(PointID, DeviceName, ItemID);
            }
            else
            {
                // 获取分页数据
                int pageIndex = Convert.ToInt32(strPageIndex);
                int pageSize = Convert.ToInt32(strPageSize);
                pager = new Pager(pageIndex, pageSize, this.repository.GetOriginalCount(PointID, DeviceName, ItemID));
                points = this.repository.GetOriginalPagerItems(PointID, DeviceName, ItemID, pageIndex, pageSize, u => u.PointID);
            }

            var items = from item in points
                        select new
                        {
                            ID = item.ID,
                            PointID = item.PointID,
                            ModuleID = item.ModuleID,
                            Protocol = item.Protocol,
                            AreaID = item.AreaID,
                            Floor = item.Floor,
                            ItemID = item.ItemID,
                            ItemName = item.ItemName,
                            ItemDescription = item.ItemDescription,
                            Scope = item.Scope,
                            DeviceName = item.DeviceName,
                            PhyAddr = item.PhyAddr,
                            ValueFunc = item.ValueFunc,
                            MinValue = item.MinValue,
                            MaxValue = item.MaxValue,
                            Type = item.Type,
                            Unit = item.Unit,
                            IsArchive = item.IsArchive,
                            ArchiveInterval = item.ArchiveInterval,
                            ParentID = item.ParentID,
                            Remark1 = item.Remark1,
                            Remark2 = item.Remark2,
                            Remark3 = item.Remark3
                        };
            pager.Items = items;

            return Ok(pager);
        }

        // GET: api/points/1
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> GetPoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point item = await this.repository.GetByIdAsync(uuid);
            if (item == null)
                return NotFound();

            var result = new
            {
                ID = item.ID,
                PointID = item.PointID,
                ModuleID = item.ModuleID,
                Protocol = item.Protocol,
                AreaID = item.AreaID,
                Floor = item.Floor,
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                Scope = item.Scope,
                DeviceName = item.DeviceName,
                PhyAddr = item.PhyAddr,
                ValueFunc = item.ValueFunc,
                MinValue = item.MinValue,
                MaxValue = item.MaxValue,
                Type = item.Type,
                Unit = item.Unit,
                IsArchive = item.IsArchive,
                ArchiveInterval = item.ArchiveInterval,
                ParentID = item.ParentID,
                Remark1 = item.Remark1,
                Remark2 = item.Remark2,
                Remark3 = item.Remark3
            };

            return Ok(result);
        }

        // PUT: api/points/400001
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPoint(int uuid, [FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            if (uuid != point.ID)
                return BadRequest();

            try
            {
                await this.repository.PutAsync(point);
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

        // POST: api/points
        [ResponseType(typeof(Point))]
        public async Task<IHttpActionResult> PostPoint([FromUri]Point point)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            try
            {
                point.TopPos = 0;
                point.LeftPos = 0;
                point.Status = 0;
                point.DateTime = DateTime.Now;
                point.ArchiveTime = DateTime.Now;
                point.ArchiveTag = false;
                await this.repository.AddAsync(point);
            }
            catch (DbUpdateException)
            {
                if (this.repository.IsExist(point.ID))
                    return Conflict();
                else
                    throw;
            }

            return Ok();
        }

        // DELETE: api/points/400001
        public async Task<IHttpActionResult> DeletePoint(int uuid)
        {
            var errResult = TextHelper.CheckAuthorized(Request);
            if (errResult != null)
                return errResult;

            Point point = await this.repository.GetByIdAsync(uuid);
            if (point == null)
                return NotFound();

            await this.repository.DeleteAsync(point);

            return Ok();
        }
    }
}
