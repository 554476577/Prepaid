using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Prepaid.Utils
{
    public class XmlSerializeHelper
    {
        public static void SaveToXml<T>(string filePath, T sourceObj)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && sourceObj != null)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(sourceObj.GetType());
                    xmlSerializer.Serialize(writer, sourceObj);
                }
            }
        }

        public static T LoadFromXml<T>(string filePath) where T : class
        {
            T result = null;

            if (!File.Exists(filePath))
                throw new ArgumentException("文件不存在！");

            using (StreamReader reader = new StreamReader(filePath))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                result = xmlSerializer.Deserialize(reader) as T;
            }

            return result;
        }
    }
}
