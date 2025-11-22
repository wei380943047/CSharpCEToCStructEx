using System.Collections.Generic;
using System.Xml.Linq;

namespace StructEx
{
    public class StructNode(string name)
    {
        public string Name { get; set; } = name;
        public List<FieldNode> Fields { get; set; } = [];
        public bool ShouldGenerate { get; set; } = true;
        public int TotalSize { get; set; }
        public XElement? XmlElement { get; set; }

     

        // 新增：更新 XML
        public void UpdateXml()
        {
            XmlElement?.SetAttributeValue("Name", Name);
        }
    }
}