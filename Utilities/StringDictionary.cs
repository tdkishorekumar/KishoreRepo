using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SearchFight.Utilities
{
    [XmlRoot("StringDictionary")]
    public class StringDictionary : Dictionary<string, string>, IXmlSerializable
    {
        public StringDictionary() : base() { }

        public StringDictionary(Dictionary<string, string> dictionary)
            : base(dictionary) { }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(string));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (!reader.IsStartElement("Item"))
                    throw new InvalidOperationException("Header element expected.");

                var name = reader.GetAttribute("Name");
                var value = reader.GetAttribute("Value");
                this.Add(name, value);

                reader.Read();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(string));

            foreach (var kvp in this)
            {
                writer.WriteStartElement("Item");

                writer.WriteAttributeString("Name", kvp.Key);
                writer.WriteAttributeString("Value", kvp.Value);

                writer.WriteEndElement();
            }
        }
    }
}