using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace StructEx
{
    public class StructParser(string filePath)
    {
        private readonly XDocument _doc = XDocument.Load(filePath);
        private readonly string _filePath = filePath;

        // ========================================
        // ✅ 新增：C# 类型到 C++ 类型的映射表
        // ========================================
        private static readonly Dictionary<string, string> CSharpTypeMappings = new()
        {
            // 泛型集合
            { "List`1", "CListVoid*" },
            { "Dictionary`2", "CDictionaryVoid*" },
            { "HashSet`1", "CHashSetVoid*" },
            { "Queue`1", "CQueueVoid*" },
            { "Stack`1", "CStackVoid*" },
            
            // 并发集合
            { "ConcurrentDictionary`2", "CConcurrentDictionary*" },
            { "ConcurrentQueue`1", "CConcurrentQueue*" },
            { "ConcurrentBag`1", "CConcurrentBag*" },
            
            // 特殊集合
            { "LinkedList`1", "CLinkedList*" },
            { "SortedDictionary`2", "CSortedDictionary*" },
            { "SortedList`2", "CSortedList*" },
            
            // 基本类型
            { "String", "MonoString*" },
            { "Object", "MonoObject*" },
            { "Int32", "int32_t" },
            { "Int64", "int64_t" },
            { "UInt32", "uint32_t" },
            { "UInt64", "uint64_t" },
            { "Single", "float" },
            { "Double", "double" },
            { "Boolean", "bool" },
            { "Byte", "uint8_t" },
            { "Int16", "int16_t" },
            { "UInt16", "uint16_t" },
            { "Char", "wchar_t" },
        };

        public List<StructNode> Parse()
        {
            var structures = new List<StructNode>();
            var structElements = _doc.Descendants("Structure").Where(e => e.Parent.Name == "Structures");

            foreach (var element in structElements)
            {
                var structNode = ParseStruct(element);
                if (structNode != null)
                {
                    structures.Add(structNode);
                }
            }

            return structures;
        }

        private StructNode? ParseStruct(XElement element)
        {
            var name = element.Attribute("Name")?.Value;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var structNode = new StructNode(name)
            {
                XmlElement = element
            };

            // ✅ 新增：检查是否是已知 C# 类型
            var mappedType = GetCSharpTypeMapping(name);
            if (mappedType != null)
            {
                structNode.ShouldGenerate = false;  // 标记为不生成
            }

            var elements = element.Element("Elements")?.Elements("Element");

            if (elements != null)
            {
                foreach (var elem in elements)
                {
                    var field = ParseField(elem);
                    if (field != null)
                    {
                        structNode.Fields.Add(field);
                    }
                }
            }

            if (structNode.Fields.Any())
            {
                var lastField = structNode.Fields.OrderBy(f => f.Offset).Last();
                structNode.TotalSize = lastField.Offset + lastField.ByteSize;
            }

            return structNode;
        }

        private FieldNode? ParseField(XElement element)
        {
            var offset = int.Parse(element.Attribute("Offset")?.Value ?? "0");
            var vartype = element.Attribute("Vartype")?.Value ?? "Pointer";
            var bytesize = int.Parse(element.Attribute("Bytesize")?.Value ?? "8");
            var description = element.Attribute("Description")?.Value ?? $"field_{offset:X4}";

            var cleanedName = CleanFieldName(description);

            var field = new FieldNode
            {
                Name = cleanedName,
                OriginalDescription = description,  // ✅ 新增：保存原始描述
                Offset = offset,
                OriginalType = vartype,
                ByteSize = bytesize,
                CppType = MapToCppType(vartype, bytesize, cleanedName),
                XmlElement = element  // ✅ 新增：保存 XML 元素引用
            };

            var childStruct = element.Element("Structure");
            if (childStruct == null) return field;
            field.ChildStruct = ParseStruct(childStruct);

            if (field.ChildStruct == null) return field;
            // ✅ 修改：检查是否是已知的 C# 类型
            var mappedType = GetCSharpTypeMapping(field.ChildStruct.Name);
            field.CppType = mappedType ?? $"{field.ChildStruct.Name}*";
            field.UseSpecificType = true;

            return field;
        }

        // ========================================
        // ✅ 新增：获取 C# 类型映射
        // ========================================
        private static string? GetCSharpTypeMapping(string typeName)
        {
            // 精确匹配
            if (CSharpTypeMappings.TryGetValue(typeName, out string? mappedType))
            {
                return mappedType;
            }

            // 模糊匹配（处理泛型）
            if (Regex.IsMatch(typeName, @"^List`\d+$"))
                return "CListVoid*";

            if (Regex.IsMatch(typeName, @"^Dictionary`\d+$"))
                return "CDictionaryVoid*";

            if (Regex.IsMatch(typeName, @"^HashSet`\d+$"))
                return "CHashSetVoid*";

            if (Regex.IsMatch(typeName, @"^Queue`\d+$"))
                return "CQueueVoid*";

            if (Regex.IsMatch(typeName, @"^Stack`\d+$"))
                return "CStackVoid*";

            // 数组
            if (typeName.EndsWith("[]"))
                return "CArrayVoid*";

            // 未知类型返回 null
            return null;
        }

        private string CleanFieldName(string name)
        {
            name = name.Replace("&lt;", "").Replace("&gt;", "")
                       .Replace("&amp;", "&")
                       .Replace("<", "").Replace(">", "")
                       .Replace("k__BackingField", "");
            name = Regex.Replace(name, @"\s+", "_");
            name = Regex.Replace(name, @"[^\w]", "_");
            name = Regex.Replace(name, @"`\d+", "");
            name = name.Trim('_');
            return string.IsNullOrEmpty(name) ? "unknown" : name;
        }

        private string MapToCppType(string vartype, int bytesize, string fieldName)
        {
            if (vartype == "Pointer")
                return "void*";

            // ✅ 新增：检查是否是已知的 C# 类型
            var mappedType = GetCSharpTypeMapping(vartype);
            if (mappedType != null)
            {
                return mappedType;
            }

            if (vartype.Contains("Float") || vartype.Contains("Single"))
                return "float";

            if (vartype.Contains("Double"))
                return "double";

            if (bytesize == 8)
            {
                if (Regex.IsMatch(fieldName, @"(Id|FrameId)$"))
                    return "int64_t";
                return "uint64_t";
            }

            if (bytesize == 4)
            {
                if (Regex.IsMatch(fieldName, @"(Id|Index|Count|Year|Month|Level|State|Point|Grade|Size|Length)$"))
                    return "int32_t";
                return "uint32_t";
            }

            if (bytesize == 2)
                return "uint16_t";

            if (bytesize == 1)
            {
                if (Regex.IsMatch(fieldName, @"^(is|has|can|Enable|Disable|Open|Unlock)"))
                    return "bool";
                return "uint8_t";
            }

            return "void*";
        }

        // ✅ 新增：保存到 XML
        public void SaveChanges()
        {
            _doc.Save(_filePath);
        }

        public void SaveChangesAs(string newFilePath)
        {
            _doc.Save(newFilePath);
        }
    }
}