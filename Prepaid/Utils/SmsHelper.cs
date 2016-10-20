using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace Prepaid.Utils
{
    public class SmsHelper
    {
        private static Configuration m_config =
            WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);

        public static bool Send(string phone, string data)
        {
            bool isSuccess = true;
            try
            {
                string url = m_config.AppSettings.Settings["SmsUrl"].Value;
                string appkey = m_config.AppSettings.Settings["SmsAppkey"].Value;
                string secret = m_config.AppSettings.Settings["SmsSecret"].Value; ;
                ITopClient client = new DefaultTopClient(url, appkey, secret);
                AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
                req.Extend = "";
                req.SmsType = "normal";
                req.SmsFreeSignName = "预付费管理系统";
                req.SmsParam = data;
                req.RecNum = phone;
                req.SmsTemplateCode = "SMS_20160057";
                AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
                Match match = Regex.Match(rsp.Body, "<success>(.*?)</success>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                    isSuccess = Convert.ToBoolean(match.Groups[1].Value);
            }
            catch
            {
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}