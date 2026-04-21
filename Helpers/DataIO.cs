using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace _90s_Minimalism_CMS_Project.Helpers
{
    public class DataIO
    {
        public void SerializeObject<T>(T serializableObject, string fileName)
        {
            if(serializableObject == null)
            {
                return;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());

                using(MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDoc.Load(stream);
                    xmlDoc.Save(fileName);
                    stream.Close();
                }
            }
            catch(Exception)
            {
                //Log exception here
            }
        }

        public T DeSerializeObject<T>(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return default(T);
            }
            T objectOut = default(T);
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                string xmlString = xmlDoc.OuterXml;

                using(StringReader read = new StringReader(xmlString))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using(XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    read.Close();
                }
            }
            catch(Exception)
            {
                //Log exception here
            }
            return objectOut;
        }
    }
}
