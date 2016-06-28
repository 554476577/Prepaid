using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class UserEnergy
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 归档时间
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// 该用户下面拥有的仪表的总刻度值
        /// </summary>
        public double SumTotolValue { get; set; }

        /// <summary>
        /// 该用户最近一次交费时的所用能耗值
        /// </summary>
        public double SumValue { get; set; }

        /// <summary>
        /// 该用户最近一次交费时的所交金额费用
        /// </summary>
        public int? SumMoney { get; set; }

        /// <summary>
        /// 该用户下面拥有的所有仪表的能耗状况
        /// </summary>
        public IEnumerable<DeviceEnergy> DeviceEnergies { get; set; }
    }
}