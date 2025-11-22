namespace StructEx
{
    partial class KanWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KanWindow));
            FileLabel = new Label();
            txtFilePath = new TextBox();
            btnOpenFile = new Button();
            btnGenerate = new Button();
            treeStructures = new TreeView();
            panel1 = new Panel();
            btnApply = new Button();
            chkGenerate = new CheckBox();
            txtComment = new TextBox();
            cmbCppTypeLabel = new Label();
            cmbCppType = new ComboBox();
            label3 = new Label();
            txtOffset = new TextBox();
            label2 = new Label();
            txtFieldName = new TextBox();
            label1 = new Label();
            label5 = new Label();
            txtPreview = new TextBox();
            btnSaveXml = new Button();
            btnSaveXmlAs = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // FileLabel
            // 
            FileLabel.AutoSize = true;
            FileLabel.Location = new Point(11, 18);
            FileLabel.Name = "FileLabel";
            FileLabel.Size = new Size(44, 24);
            FileLabel.TabIndex = 0;
            FileLabel.Text = "File:";
            // 
            // txtFilePath
            // 
            txtFilePath.BorderStyle = BorderStyle.FixedSingle;
            txtFilePath.Location = new Point(65, 14);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(572, 30);
            txtFilePath.TabIndex = 1;
            // 
            // btnOpenFile
            // 
            btnOpenFile.Location = new Point(654, 13);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(112, 34);
            btnOpenFile.TabIndex = 2;
            btnOpenFile.Text = "Open";
            btnOpenFile.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(776, 14);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(112, 34);
            btnGenerate.TabIndex = 3;
            btnGenerate.Text = "Generate C++";
            btnGenerate.UseVisualStyleBackColor = true;
            // 
            // treeStructures
            // 
            treeStructures.CheckBoxes = true;
            treeStructures.Location = new Point(7, 66);
            treeStructures.Name = "treeStructures";
            treeStructures.Size = new Size(906, 679);
            treeStructures.TabIndex = 4;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnApply);
            panel1.Controls.Add(chkGenerate);
            panel1.Controls.Add(txtComment);
            panel1.Controls.Add(cmbCppTypeLabel);
            panel1.Controls.Add(cmbCppType);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(txtOffset);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(txtFieldName);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(919, 66);
            panel1.Name = "panel1";
            panel1.Size = new Size(345, 679);
            panel1.TabIndex = 6;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(48, 290);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(191, 41);
            btnApply.TabIndex = 9;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // chkGenerate
            // 
            chkGenerate.AutoSize = true;
            chkGenerate.Checked = true;
            chkGenerate.CheckState = CheckState.Checked;
            chkGenerate.Location = new Point(48, 230);
            chkGenerate.Name = "chkGenerate";
            chkGenerate.Size = new Size(193, 28);
            chkGenerate.TabIndex = 8;
            chkGenerate.Text = "Generate this field";
            chkGenerate.UseVisualStyleBackColor = true;
            // 
            // txtComment
            // 
            txtComment.BorderStyle = BorderStyle.FixedSingle;
            txtComment.Location = new Point(141, 160);
            txtComment.Name = "txtComment";
            txtComment.Size = new Size(182, 30);
            txtComment.TabIndex = 7;
            // 
            // cmbCppTypeLabel
            // 
            cmbCppTypeLabel.AutoSize = true;
            cmbCppTypeLabel.Location = new Point(28, 160);
            cmbCppTypeLabel.Name = "cmbCppTypeLabel";
            cmbCppTypeLabel.Size = new Size(99, 24);
            cmbCppTypeLabel.TabIndex = 6;
            cmbCppTypeLabel.Text = "Comment:";
            // 
            // cmbCppType
            // 
            cmbCppType.DisplayMember = "0";
            cmbCppType.FormattingEnabled = true;
            cmbCppType.Location = new Point(141, 110);
            cmbCppType.Name = "cmbCppType";
            cmbCppType.Size = new Size(182, 32);
            cmbCppType.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(28, 113);
            label3.Name = "label3";
            label3.Size = new Size(99, 24);
            label3.TabIndex = 4;
            label3.Text = "C++ Type:";
            // 
            // txtOffset
            // 
            txtOffset.BorderStyle = BorderStyle.FixedSingle;
            txtOffset.Location = new Point(141, 60);
            txtOffset.Name = "txtOffset";
            txtOffset.ReadOnly = true;
            txtOffset.Size = new Size(184, 30);
            txtOffset.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(61, 66);
            label2.Name = "label2";
            label2.Size = new Size(66, 24);
            label2.TabIndex = 2;
            label2.Text = "Offset:";
            // 
            // txtFieldName
            // 
            txtFieldName.BorderStyle = BorderStyle.FixedSingle;
            txtFieldName.Location = new Point(141, 17);
            txtFieldName.Name = "txtFieldName";
            txtFieldName.Size = new Size(184, 30);
            txtFieldName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 18);
            label1.Name = "label1";
            label1.Size = new Size(113, 24);
            label1.TabIndex = 0;
            label1.Text = "Field Name:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 757);
            label5.Name = "label5";
            label5.Size = new Size(80, 24);
            label5.TabIndex = 7;
            label5.Text = "Preview:";
            // 
            // txtPreview
            // 
            txtPreview.BackColor = Color.WhiteSmoke;
            txtPreview.BorderStyle = BorderStyle.FixedSingle;
            txtPreview.Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPreview.Location = new Point(111, 751);
            txtPreview.Multiline = true;
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.ScrollBars = ScrollBars.Both;
            txtPreview.Size = new Size(1148, 513);
            txtPreview.TabIndex = 8;
            // 
            // btnSaveXml
            // 
            btnSaveXml.Location = new Point(905, 14);
            btnSaveXml.Name = "btnSaveXml";
            btnSaveXml.Size = new Size(112, 34);
            btnSaveXml.TabIndex = 9;
            btnSaveXml.Text = "Save XML";
            btnSaveXml.UseVisualStyleBackColor = true;
            btnSaveXml.Click += BtnSaveXml_Click;
            // 
            // btnSaveXmlAs
            // 
            btnSaveXmlAs.Location = new Point(1033, 14);
            btnSaveXmlAs.Name = "btnSaveXmlAs";
            btnSaveXmlAs.Size = new Size(153, 34);
            btnSaveXmlAs.TabIndex = 10;
            btnSaveXmlAs.Text = "Save XML As...";
            btnSaveXmlAs.UseVisualStyleBackColor = true;
            btnSaveXmlAs.Click += BtnSaveXmlAs_Click;
            // 
            // KanWindow
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1271, 1264);
            Controls.Add(btnSaveXmlAs);
            Controls.Add(btnSaveXml);
            Controls.Add(txtPreview);
            Controls.Add(label5);
            Controls.Add(panel1);
            Controls.Add(treeStructures);
            Controls.Add(btnGenerate);
            Controls.Add(btnOpenFile);
            Controls.Add(txtFilePath);
            Controls.Add(FileLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "KanWindow";
            Text = "KanCEStructToC++";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label FileLabel;
        private TextBox txtFilePath;
        private Button btnOpenFile;
        private Button btnGenerate;
        private TreeView treeStructures;
        private Panel panel1;
        private Button btnApply;
        private CheckBox chkGenerate;
        private TextBox txtComment;
        private Label cmbCppTypeLabel;
        private ComboBox cmbCppType;
        private Label label3;
        private TextBox txtOffset;
        private Label label2;
        private TextBox txtFieldName;
        private Label label1;
        private Label label5;
        private TextBox txtPreview;
        private Button btnSaveXml;
        private Button btnSaveXmlAs;
    }
}
