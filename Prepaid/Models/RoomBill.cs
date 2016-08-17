using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class RoomBill
    {
        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 结算批次号
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// 该房间内部所有设备总能耗值
        /// </summary>
        public double SumValue { get; set; }

        /// <summary>
        /// 该房间内部所有设备总能耗费用
        /// </summary>
        public string SumMoney { get; set; }

        /// <summary>
        /// 该房间内部所有设备的能耗详情
        /// </summary>
        public IEnumerable<DeviceBill> DeviceBills { get; set; }
    }
}