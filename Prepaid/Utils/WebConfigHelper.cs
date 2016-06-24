using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Prepaid.Utils
{
    public class WebConfigHelper
    {
        private static Configuration mConfig =
            WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
        private const string PROVIDER_NAME = "System.Data.SqlClient";

        /// <summary>
        /// 写入appSettings
        /// </summary>
        /// <param name="item">appSettings等</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteAppSetting(string key, string value)
        {
            AppSettingsSection appSection = mConfig.AppSettings;
            if (appSection.Settings[key] == null)
            {
                appSection.Settings.Add(key, value);
                mConfig.Save();
            }
            else
            {
                appSection.Settings.Remove(key);
                appSection.Settings.Add(key, value);
                mConfig.Save();
            }
        }

        /// <summary>
        /// 获取指定Key的appSetting值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppSetting(string key)
        {
            var element = mConfig.AppSettings.Settings[key];
            return element == null ? string.Empty : element.Value;
        }

        /// <summary>
        /// 写入数据库连接字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteConnectString(string key, string value)
        {
            ConnectionStringsSection section = mConfig.ConnectionStrings;
            if (section.ConnectionStrings[key] == null)
            {
                section.ConnectionStrings.Add(new ConnectionStringSettings(key, value,PROVIDER_NAME));
                mConfig.Save();
            }
            else
            {
                section.ConnectionStrings.Remove(key);
                section.ConnectionStrings.Add(new ConnectionStringSettings(key, value, PROVIDER_NAME));
                mConfig.Save();
            }
        }

        /// <summary>
        /// 读取数据库连接字符串
        /// </summary>
        /// <param name="key"></param>
        public static string ReadConnectString(string key)
        {
            ConnectionStringSettings settings = mConfig.ConnectionStrings.ConnectionStrings[key];
            return settings == null ? string.Empty : settings.ConnectionString;
        }

        // 测试数据库连接
        public static bool TestDbConnect(string connectString)
        {
            bool isSuccess = false;
            SqlConnection connection = new SqlConnection(connectString);
            try
            {
                connection.Open();
                if (connection.State != ConnectionState.Closed && connection.State != ConnectionState.Broken)
                    isSuccess = true;
            }
            catch
            {
                isSuccess = false;
            }
            finally
            {
                connection.Close();
            }
                
            return isSuccess;
        }

        /// <summary>
        /// 解析连接字符。用一个键值对的字典结构存储。
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ResolveConnectString(string connectString)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] strAry = connectString.Split(';');
            foreach (string item in strAry)
            {
                string[] pair = item.Split('=');
                if (pair.Length != 2)
                    continue;
                string key = pair[0];
                string value = pair[1];
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
            }

            return dict;
        }
    }
}