using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class PrepaidEnergy
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
        /// 所在建筑名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 所在房间编号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public string AccountBalance { get; set; }

        /// <summary>
        /// 账户报警限制
        /// </summary>
        public string AccountWarnLimit { get; set; }

        /// <summary>
        /// 账户积分
        /// </summary>
        public int? CreditScore { get; set; }

        /// <summary>
        /// 最高可拖欠金额
        /// </summary>
        public string MaxArrears { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的能耗总量
        /// </summary>
        public double CurrentSumValue { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的总金额(扩大100倍)
        /// </summary>
        public int CurrentSumMoney { get; set; }

        /// <summary>
        /// 用户在当前时刻消耗的总金额
        /// </summary>
        public string StrCurrentSumMoney { get; set; }

        /// <summary>
        /// 用户在当前时刻的账户余额(扩大100倍)
        /// </summary>
        public int? CurrentAccountBalance { get; set; }

        /// <summary>
        /// 用户在当前时刻的账户余额
        /// </summary>
        public string StrCurrentAccountBalance { get; set; }

        /// <summary>
        /// 该用户在当前时刻拥有的所有仪表的能耗状况
        /// </summary>
        public IEnumerable<InstantDeviceEnergy> InstantDeviceEnergies { get; set; }
    }
}