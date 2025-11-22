using System.Xml.Linq;

namespace StructEx
{
    public class FieldNode
    {
        public required string Name { get; set; }
        public int Offset { get; set; }
        public string? OriginalDescription { get; set; } 
        public required string OriginalType { get; set; }
        public int ByteSize { get; set; }
        public required string CppType { get; set; }
        public string? Comment { get; set; }
        public bool ShouldGenerate { get; set; } = true;
        public bool UseSpecificType { get; set; } = false;
        public StructNode? ChildStruct { get; set; }
        public XElement? XmlElement { get; set; }


        public void UpdateXml()
        {
            XmlElement?.SetAttributeValue("Description", Name);
        }
    }
}