using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class PrepaidDeviceBill
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
        /// 上一次抄表读数
        /// </summary>
        public double? PreValue { get; set; }

        /// <summary>
        /// 当前时刻读数
        /// </summary>
        public double? CurValue { get; set; }

        /// <summary>
        /// 格式化之后的单价
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public int IntMoney { get; set; }

        /// <summary>
        /// 格式化之后的价格
        /// </summary>
        public string Money { get; set; }
    }
}