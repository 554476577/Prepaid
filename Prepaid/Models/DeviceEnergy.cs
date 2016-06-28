using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class DeviceEnergy
    {
        /// <summary>
        /// 点位ID
        /// </summary>
        public int PointID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备总读数
        /// </summary>
        public double TotolValue { get; set; }

        /// <summary>
        /// 最近一次交费时的所用的设备能耗值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 最近一次交费时的所交金额费用
        /// </summary>
        public int? Money { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}