using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;

namespace Prepaid.Utils
{
    public class EmailHelper
    {
        private MailMessage m_mail;
        private Configuration m_config =
            WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);

        public EmailHelper(string toAddr, string subject, string body)
        {
            m_mail = new MailMessage();
            m_mail.From = new MailAddress(m_config.AppSettings.Settings["EmailUserName"].Value);
            m_mail.To.Add(new MailAddress(toAddr));
            m_mail.Subject = subject;
            m_mail.IsBodyHtml = true;
            m_mail.BodyEncoding = System.Text.Encoding.UTF8;
            m_mail.Body = body;
        }

        ///<summary>
        /// 添加附件
        ///</summary>
        ///<param name="attachPaths">附件的路径集合，以分号分隔</param>
        public bool AddAttachments(string attachPaths)
        {
            bool isSuccess = true;
            try
            {
                string[] paths = attachPaths.Split(';'); //以什么符号分隔可以自定义
                Attachment attachment;
                ContentDisposition disposition;
                foreach (string path in paths)
                {
                    if (!File.Exists(path))
                        continue;
                    attachment = new Attachment(path, MediaTypeNames.Application.Octet);
                    attachment.Name = System.IO.Path.GetFileName(path);
                    attachment.NameEncoding = System.Text.Encoding.GetEncoding("gb2312");
                    attachment.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    disposition = attachment.ContentDisposition;
                    disposition.Inline = true;
                    disposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                    disposition.CreationDate = File.GetCreationTime(path);
                    disposition.ModificationDate = File.GetLastWriteTime(path);
                    disposition.ReadDate = File.GetLastAccessTime(path);
                    this.m_mail.Attachments.Add(attachment);
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool Send()
        {
            bool isSuccess = true;

            try
            {
                SmtpClient client = new SmtpClient(m_config.AppSettings.Settings["SmtpHost"].Value, 25);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Timeout = 60000 * 5;
                string userName = m_config.AppSettings.Settings["EmailUserName"].Value;
                string password = m_config.AppSettings.Settings["EmailPassword"].Value;
                client.Credentials = new System.Net.NetworkCredential(userName, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(this.m_mail);
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            finally
            {
                this.m_mail.Dispose();
            }

            return isSuccess;
        }
    }
}