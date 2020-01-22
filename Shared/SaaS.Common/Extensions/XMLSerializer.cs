using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SaaS.Common.Extensions
{
    public class XMLSerializer
    {
        public static string SerializeObject<T>(T obj) where T : new()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var xs = new XmlSerializer(typeof (T));

                    var xmlwtSettings = new XmlWriterSettings();
                    xmlwtSettings.OmitXmlDeclaration = true;
                    xmlwtSettings.Encoding = Encoding.UTF8;
                    using (var xmlTextWriter = XmlWriter.Create(memoryStream, xmlwtSettings))
                    {
                        var ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        xs.Serialize(xmlTextWriter, obj, ns);
                    }
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(memoryStream))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string xml) where T : new()
        {
            XmlSerializer xs = new XmlSerializer(typeof (T));
            using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(xml)))
            {
                return (T) xs.Deserialize(memoryStream);
            }
        }
    }
}
