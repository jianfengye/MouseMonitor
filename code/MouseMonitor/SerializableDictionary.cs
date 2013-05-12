using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace MouseMonitor
{
    [XmlRoot("SerializableDictionary")]
    public class SerializableDictionary : Dictionary<String, MouseState>, IXmlSerializable
    {
        #region 构造函数
        public SerializableDictionary()
            : base()
        {
        }
        public SerializableDictionary(IDictionary<String, MouseState> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<String> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }
        public SerializableDictionary(int capacity, IEqualityComparer<String> comparer)
            : base(capacity, comparer)
        {
        }
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// 从对象的 XML 表示形式生成该对象
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(String));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(MouseState));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                String key = (String)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                MouseState value = (MouseState)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /**/
        /// <summary>
        /// 将对象转换为其 XML 表示形式
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(String));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(MouseState));
            foreach (String key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                MouseState value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
  
    }
}
