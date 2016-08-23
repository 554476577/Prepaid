using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class DeviceBill
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 上次读数
        /// </summary>
        public double PreValue { get; set; }

        /// <summary>
        /// 结算读数
        /// </summary>
        public double CurValue { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Money { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}