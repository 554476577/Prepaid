using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class PrepaidBill
    {
        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 建筑编号
        /// </summary>
        public string BuildingNo { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 信用积分
        /// </summary>
        public int? CreditScore { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public int? IntAccountBalance { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public string AccountBalance { get; set; }

        /// <summary>
        /// 可欠费金额
        /// </summary>
        public string AccountWarnLimit { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的能耗总量
        /// </summary>
        public double? SumValue { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的总金额
        /// </summary>
        public int? IntSumMoney { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的总金额(扩大100倍)
        /// </summary>
        public string SumMoney { get; set; }

        /// <summary>
        /// 平摊费用
        /// </summary>
        public int? IntApportMoney { get; set; }

        /// <summary>
        /// 用户在当前时刻需要平摊的费用(扩大100倍)
        /// </summary>
        public string ApportMoney { get; set; }

        /// <summary>
        /// 结算后账户余额
        /// </summary>
        public int? IntBilledBalance { get; set; }

        /// <summary>
        /// 结算后账户余额
        /// </summary>
        public string BilledBalance { get; set; }

        /// <summary>
        /// 该用户在当前时刻拥有的所有仪表的能耗状况
        /// </summary>
        public IEnumerable<PrepaidDeviceBill> PrepaidDeviceBills { get; set; }
    }
}