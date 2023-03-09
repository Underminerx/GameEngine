using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Underminer_Core.Tools
{
    public class SerializeHelper
    {
        /// <summary>
        /// 序列化数据并保存到文件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="path"></param>
        public static void Serialize<T>(T t, string path)
        {
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));

            using XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            dataContractJsonSerializer.WriteObject(writer, t);
        
        }

        public static void DeSerialize<T> (string path, out T? t)       // 读取失败 out的t=null
        {
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
            using XmlTextReader reader = new XmlTextReader(path);
            t = (T?)dataContractJsonSerializer.ReadObject(reader);
        }
    }
}
