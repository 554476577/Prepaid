using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    /// <summary>
    /// 分页模型。
    /// </summary>
    public class Pager
    {
        public Pager() { }

        public Pager(int pageIndex, int pageSize, int recordCount)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.RecordCount = recordCount;

            // 计算总页数
            if (this.RecordCount % this.PageSize == 0)
                this.PageCount = this.RecordCount / this.PageSize;
            else
                this.PageCount = this.RecordCount / this.PageSize + 1;

            // 计算数据开始编号
            this.RecordStart = this.PageSize * (this.PageIndex - 1) + 1;
        }

        /// <summary>
        /// 页号（请求第几页的数据）,由1开始。
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 每页展示多少条数据。
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 页数。
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 记录总数。
        /// </summary>
        public int RecordCount { get; private set; }

        /// <summary>
        /// 数据开始编号，由1开始。
        /// </summary>
        public int RecordStart { get; private set; }

        /// <summary>
        /// 展示的数据列表。
        /// </summary>
        public IEnumerable<dynamic> Items { get; set; }
    }
}