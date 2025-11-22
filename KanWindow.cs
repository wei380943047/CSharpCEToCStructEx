using System.Text;

namespace StructEx
{
    public partial class KanWindow : Form
    {
        private List<StructNode> _structures = [];
        public readonly Dictionary<string, StructNode> StructRegistry = new Dictionary<string, StructNode>();
        private StructParser? _parser;  // 新增：保存 parser 引用
        private string? _currentFilePath;  // 新增：保存当前文件路径

        public KanWindow()
        {
            InitializeComponent();
            InitializeUi();
        }

        private void InitializeUi()
        {
            this.Text = @"CE Structure to C++ Converter";

            treeStructures.AfterSelect += TreeStructures_AfterSelect;
            treeStructures.AfterCheck += TreeStructures_AfterCheck;
            treeStructures.NodeMouseDoubleClick += TreeStructures_NodeMouseDoubleClick;

            btnOpenFile.Click += BtnOpenFile_Click;
            btnGenerate.Click += BtnGenerate_Click;
            btnApply.Click += BtnApply_Click;

            // 新增：保存按钮事件
            // 你需要在界面添加一个 btnSaveXml 按钮
            // btnSaveXml.Click += BtnSaveXml_Click;

            cmbCppType.Items.AddRange(new object[]
            {
                "void*", "int32_t", "uint32_t", "int64_t", "uint64_t",
                "float", "double", "bool", "uint8_t", "uint16_t", "wchar_t*",
                // ✅ 新增：C# 类型
                "CListVoid*", "CDictionaryVoid*", "CHashSetVoid*",
                "CQueueVoid*", "CStackVoid*", "MonoString*", "CArrayVoid*"
            });
        }

        private void BtnOpenFile_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = @"CE Structure Files|*.csx|All Files|*.*";
            dlg.Title = @"Open CE Structure File";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadStructure(dlg.FileName);
            }
        }

        private void LoadStructure(string filePath)
        {
            try
            {
                _currentFilePath = filePath;
                txtFilePath.Text = filePath;

                _parser = new StructParser(filePath);  // 保存 parser
                _structures = _parser.Parse();

                StructRegistry.Clear();
                CollectAllStructs(_structures);

                treeStructures.Nodes.Clear();

                foreach (var structNode in _structures)
                {
                    var node = BuildTreeNode(structNode, 0, new HashSet<string>());
                    treeStructures.Nodes.Add(node);
                }

                treeStructures.ExpandAll();
                UpdatePreview();

                MessageBox.Show($@"Loaded {_structures.Count} structure(s)", @"Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"""
                                 Error loading file:
                                 {ex.Message}
                                 """, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CollectAllStructs(List<StructNode> structs)
        {
            foreach (var structNode in structs)
            {
                StructRegistry.TryAdd(structNode.Name, structNode);

                foreach (var field in structNode.Fields)
                {
                    if (field.ChildStruct != null)
                    {
                        CollectAllStructs([field.ChildStruct]);
                    }
                }
            }
        }

        private static TreeNode BuildTreeNode(StructNode structNode, int depth, HashSet<string> visitedStructs)
        {
            var node = new TreeNode($"[Struct] {structNode.Name}")
            {
                Tag = structNode,
                Checked = structNode.ShouldGenerate
            };

            visitedStructs.Add(structNode.Name);

            foreach (var field in structNode.Fields.OrderBy(f => f.Offset))
            {
                var fieldText = $"{field.Name} : {field.CppType} @ 0x{field.Offset:X4}";

                var fieldNode = new TreeNode(fieldText)
                {
                    Tag = field,
                    Checked = field.ShouldGenerate
                };

                if (field.ChildStruct != null)
                {
                    if (visitedStructs.Contains(field.ChildStruct.Name))
                    {
                        var circularNode = new TreeNode($"→ [Circular Reference: {field.ChildStruct.Name}]")
                        {
                            ForeColor = Color.Gray,
                            Tag = null
                        };
                        fieldNode.Nodes.Add(circularNode);
                    }
                    else
                    {
                        fieldNode.Nodes.Add(BuildTreeNode(field.ChildStruct, depth + 1, new HashSet<string>(visitedStructs)));
                    }
                }

                node.Nodes.Add(fieldNode);
            }

            return node;
        }

        private void TreeStructures_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            switch (e.Node?.Tag)
            {
                case FieldNode field:
                    ShowFieldProperties(field);
                    break;
                case StructNode structNode:
                    ShowStructProperties(structNode);
                    break;
            }
        }

        private void ShowFieldProperties(FieldNode field)
        {
            txtFieldName.Text = field.Name;
            txtOffset.Text = $@"0x{field.Offset:X4}";
            cmbCppType.Text = field.CppType;
            txtComment.Text = field.Comment ?? "";
            chkGenerate.Checked = field.ShouldGenerate;

            txtFieldName.Enabled = true;
            cmbCppType.Enabled = true;
            txtComment.Enabled = true;
            chkGenerate.Enabled = true;
            btnApply.Enabled = true;
        }

        private void ShowStructProperties(StructNode structNode)
        {
            txtFieldName.Text = structNode.Name;
            txtOffset.Text = $@"Size: 0x{structNode.TotalSize:X}";
            cmbCppType.Text = @"struct";
            txtComment.Text = $@"{structNode.Fields.Count} fields";
            chkGenerate.Checked = structNode.ShouldGenerate;

            txtFieldName.Enabled = true;
            cmbCppType.Enabled = false;
            txtComment.Enabled = true;
            chkGenerate.Enabled = true;
            btnApply.Enabled = true;
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (treeStructures.SelectedNode?.Tag == null) return;

            switch (treeStructures.SelectedNode.Tag)
            {
                case FieldNode field:
                    field.Name = txtFieldName.Text;
                    field.CppType = cmbCppType.Text;
                    field.Comment = txtComment.Text;
                    field.ShouldGenerate = chkGenerate.Checked;

                    // 更新 XML
                    field.UpdateXml();

                    treeStructures.SelectedNode.Text =
                        $@"{field.Name} : {field.CppType} @ 0x{field.Offset:X4}";
                    break;

                case StructNode structNode:
                    {
                        var oldName = structNode.Name;
                        var newName = txtFieldName.Text;

                        if (oldName != newName)
                        {
                            structNode.Name = newName;

                            // 更新 XML
                            structNode.UpdateXml();

                            if (StructRegistry.Remove(oldName))
                            {
                                StructRegistry[newName] = structNode;
                            }

                            UpdateStructReferences(oldName, newName);
                        }

                        structNode.ShouldGenerate = chkGenerate.Checked;
                        treeStructures.SelectedNode.Text = $@"[Struct] {structNode.Name}";
                        break;
                    }
            }

            UpdatePreview();

            // 自动保存 XML
            AutoSaveXml();

            MessageBox.Show(@"Applied and saved to XML!", @"Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TreeStructures_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            switch (e.Node?.Tag)
            {
                case FieldNode field:
                    field.ShouldGenerate = e.Node.Checked;
                    break;
                case StructNode structNode:
                    structNode.ShouldGenerate = e.Node.Checked;
                    break;
            }

            UpdatePreview();
        }

        private void TreeStructures_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node?.Tag == null) return;

            switch (e.Node.Tag)
            {
                case FieldNode field:
                    {
                        var newName = Microsoft.VisualBasic.Interaction.InputBox(
                            "Enter new field name:",
                            "Rename Field",
                            field.Name
                        );

                        if (string.IsNullOrEmpty(newName)) return;

                        field.Name = newName;

                        // 更新 XML
                        field.UpdateXml();

                        e.Node.Text = $@"{field.Name} : {field.CppType} @ 0x{field.Offset:X4}";
                        UpdatePreview();

                        // 自动保存
                        AutoSaveXml();
                        break;
                    }

                case StructNode structNode:
                    {
                        var newName = Microsoft.VisualBasic.Interaction.InputBox(
                            "Enter new structure name:",
                            "Rename Structure",
                            structNode.Name
                        );

                        if (string.IsNullOrEmpty(newName)) return;

                        var oldName = structNode.Name;
                        structNode.Name = newName;

                        // 更新 XML
                        structNode.UpdateXml();

                        if (StructRegistry.Remove(oldName))
                        {
                            StructRegistry[newName] = structNode;
                        }

                        e.Node.Text = $@"[Struct] {structNode.Name}";

                        UpdateStructReferences(oldName, newName);
                        UpdatePreview();

                        // 自动保存
                        AutoSaveXml();
                        break;
                    }
            }
        }

        // ========================================
        // 新增：保存 XML 方法
        // ========================================

        private void AutoSaveXml()
        {
            try
            {
                _parser?.SaveChanges();
                this.Text = $@"CE Structure to C++ Converter - {Path.GetFileName(_currentFilePath)} [Saved]";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"""
                                 Error saving XML:
                                 {ex.Message}
                                 """, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveXml_Click(object? sender, EventArgs e)
        {
            if (_parser == null)
            {
                MessageBox.Show(@"No file loaded!", @"Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _parser.SaveChanges();
                MessageBox.Show(@"XML saved successfully!", @"Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"""
                                 Error saving XML:
                                 {ex.Message}
                                 """, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveXmlAs_Click(object? sender, EventArgs e)
        {
            if (_parser == null)
            {
                MessageBox.Show(@"No file loaded!", @"Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog();
            dlg.Filter = @"CE Structure Files|*.csx|All Files|*.*";
            dlg.DefaultExt = "csx";
            dlg.Title = @"Save CE Structure File As";

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _parser.SaveChangesAs(dlg.FileName);
                _currentFilePath = dlg.FileName;
                txtFilePath.Text = dlg.FileName;

                MessageBox.Show(@"XML saved successfully!", @"Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"""
                                 Error saving XML:
                                 {ex.Message}
                                 """, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ... 其余代码保持不变 ...

        private void UpdateStructReferences(string oldName, string newName)
        {
            foreach (var structNode in StructRegistry.Values)
            {
                foreach (var field in structNode.Fields)
                {
                    if (field.CppType == $"{oldName}*")
                    {
                        field.CppType = $"{newName}*";
                    }

                    if (field.ChildStruct?.Name == oldName)
                    {
                        field.ChildStruct.Name = newName;
                    }
                }
            }

            UpdateTreeNodeReferences(treeStructures.Nodes, oldName, newName);
        }

        private void UpdateTreeNodeReferences(TreeNodeCollection nodes, string oldName, string newName)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is FieldNode field)
                {
                    if (field.CppType == $"{oldName}*")
                    {
                        field.CppType = $"{newName}*";
                        node.Text = $@"{field.Name} : {field.CppType} @ 0x{field.Offset:X4}";
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    UpdateTreeNodeReferences(node.Nodes, oldName, newName);
                }
            }
        }

        private void UpdatePreview()
        {
            if (_structures.Count == 0)
            {
                txtPreview.Text = @"// No structure loaded";
                return;
            }

            txtPreview.Text = GenerateCpp();
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            if (_structures.Count == 0)
            {
                MessageBox.Show(@"Please load a structure file first!", @"Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog();
            dlg.Filter = @"C++ Header|*.h|All Files|*.*";
            dlg.DefaultExt = "h";
            dlg.Title = @"Save C++ Header File";

            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var code = GenerateCpp();
                File.WriteAllText(dlg.FileName, code);

                MessageBox.Show($"""
                                 Generated successfully!
                                 {dlg.FileName}
                                 """, @"Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"""
                                 Error saving file:
                                 {ex.Message}
                                 """, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateCpp()
        {
            var sb = new StringBuilder();

            sb.AppendLine("#pragma once");
            sb.AppendLine("#include \"CSharpTypes.h\"");
            sb.AppendLine();
            sb.AppendLine($"// Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"// Source: {Path.GetFileName(txtFilePath.Text)}");
            sb.AppendLine();

            HashSet<string> generated = [];

            foreach (var structNode in _structures.Where(s => s.ShouldGenerate))
            {
                GenerateStructRecursive(structNode, sb, generated);
            }

            return sb.ToString();
        }

        private static void GenerateStructRecursive(StructNode structNode, StringBuilder sb, HashSet<string> generated)
        {
            if (generated.Contains(structNode.Name))
                return;

            foreach (var field in structNode.Fields.Where(f => f.ShouldGenerate))
            {
                if (field.ChildStruct is { ShouldGenerate: true })
                {
                    GenerateStructRecursive(field.ChildStruct, sb, generated);
                }
            }

            sb.AppendLine($"// {structNode.Name}");
            sb.AppendLine($"struct {structNode.Name}");
            sb.AppendLine("{");

            var lastOffset = 0;

            foreach (var field in structNode.Fields.Where(f => f.ShouldGenerate).OrderBy(f => f.Offset))
            {
                if (field.Offset > lastOffset)
                {
                    var padSize = field.Offset - lastOffset;
                    sb.AppendLine($"    char _pad_{lastOffset:X4}[{padSize}];{new string(' ', 20)}// 0x{lastOffset:X4}");
                }

                var comment = string.IsNullOrEmpty(field.Comment) ? "" : $" /* {field.Comment} */";
                sb.AppendLine($"    {field.CppType,-20} {field.Name,-40};{comment} // 0x{field.Offset:X4}");

                lastOffset = field.Offset + field.ByteSize;
            }

            sb.AppendLine("};");
            sb.AppendLine($"static_assert(sizeof({structNode.Name}) == 0x{lastOffset:X}, \"Size check\");");
            sb.AppendLine();

            generated.Add(structNode.Name);
        }
    }
}