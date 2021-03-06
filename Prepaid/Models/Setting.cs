﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    /// <summary>
    /// 消息通知方式
    /// </summary>
    public enum NotifyMode
    {
        /// <summary>
        /// 短信
        /// </summary>
        Sms = 0,
        /// <summary>
        /// 邮件
        /// </summary>
        Email,
        /// <summary>
        /// 微信
        /// </summary>
        Wechat,
        /// <summary>
        /// 电话
        /// </summary>
        Phone
    }

    /// <summary>
    /// 定时结算时间间隔
    /// </summary>
    public enum TimingSettleMode
    {
        /// <summary>
        /// 一天
        /// </summary>
        Day = 0,
        /// <summary>
        /// 一周
        /// </summary>
        Week,
        /// <summary>
        /// 半个月
        /// </summary>
        HalfMonth,
        /// <summary>
        /// 一个月
        /// </summary>
        Month
    }

    public class Setting
    {
        //private NotifyMode notifyMode = NotifyMode.Wechat;
        private bool isTimingSettle = false;
        private TimingSettleMode settleInterval = TimingSettleMode.Month;
        private bool isRechargeSettle = false;
        private int rechargeLimitInterval = 5;

        /// <summary>
        /// 通知方式 0:短信 1:邮件 2:微信 3:电话
        /// </summary>
        public string Notify { get; set; }

        /// <summary>
        /// 是否开启定时结算
        /// </summary>
        public bool IsTimingSettle
        {
            get { return isTimingSettle; }
            set { isTimingSettle = value; }
        }

        /// <summary>
        /// 定时结算时间间隔
        /// </summary>
        public TimingSettleMode SettleInterval
        {
            get { return settleInterval; }
            set { settleInterval = value; }
        }

        /// <summary>
        /// 是否开启有效充值时间设定
        /// </summary>
        public bool IsRechargeSettle
        {
            get { return isRechargeSettle; }
            set { isRechargeSettle = value; }
        }

        /// <summary>
        /// 再次充值时间间隔
        /// </summary>
        public int RechargeLimitInterval
        {
            get { return rechargeLimitInterval; }
            set { rechargeLimitInterval = value; }
        }
    }
}