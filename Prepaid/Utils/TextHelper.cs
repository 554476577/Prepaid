using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Prepaid.Utils
{
    public class TextHelper
    {
        /// <summary>
        /// 生成一个32位的UUID
        /// </summary>
        /// <returns></returns>
        public static string GenerateUUID()
        {
            string uuid = Guid.NewGuid().ToString();
            uuid = uuid.Replace("-", "");
            return uuid;
        }

        ///   <summary>  
        ///   给一个字符串进行MD5加密  
        ///   </summary>  
        ///   <param   name="strText">待加密字符串</param>  
        ///   <returns>加密后的字符串</returns>  
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strText);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        /// <summary>
        /// 判断用户是否登录/签到
        /// </summary>
        /// <returns></returns>
        public static bool IsSignin()
        {
            var userInfo = HttpContext.Current.Session["mySession"] as Prepaid.Models.UserSession;
            return userInfo == null ? false : true;
        }

        /// <summary>
        /// 检验用户是否已经授权访问系统
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static System.Web.Http.IHttpActionResult CheckAuthorized(System.Net.Http.HttpRequestMessage request)
        {
            if (!IsSignin())
            {
                var errorResult = new Prepaid.Results.InternalServerErrorTextPlainResult("未授权访问，请先登录！", request);
                errorResult.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                return errorResult;
            }

            return null;
        }

        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetHostAddress()
        {
            string userHostAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(userHostAddress))
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
                return userHostAddress;

            return "127.0.0.1"; ;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}