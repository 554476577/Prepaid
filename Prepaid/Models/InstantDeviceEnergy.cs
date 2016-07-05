using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class InstantDeviceEnergy
    {
        /// <summary>
        /// 点位ID
        /// </summary>
        public int PointID { get; set; }

        /// <summary>
        /// 设备联动ID
        /// </summary>
        public int DeviceLinkID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 上一次抄表时间
        /// </summary>
        public DateTime? PreDateTime { get; set; }

        /// <summary>
        /// 上一次抄表读数
        /// </summary>
        public double PreValue { get; set; }

        /// <summary>
        /// 当前时刻读数
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// 距离最近一次抄表时间的用能数
        /// </summary>
        public double IntervalValue { get; set; }

        /// <summary>
        /// 距离最近一次抄表时间的能耗金额
        /// </summary>
        public int IntervalMoney { get; set; }
    }
}