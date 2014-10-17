namespace Xuld.JsonFormator {
    partial class MainForm {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            Xuld.JsonFormator.Properties.Settings settings1 = new Xuld.JsonFormator.Properties.Settings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.miPasteAndFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.miFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.miStringify = new System.Windows.Forms.ToolStripMenuItem();
            this.miCollapse = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miHomepage = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.cbUseStrictMode = new System.Windows.Forms.CheckBox();
            this.tvImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpSource = new System.Windows.Forms.TabPage();
            this.cmEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmiFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiPastle = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiPastleAndFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.tpObject = new System.Windows.Forms.TabPage();
            this.tvOjectTree = new System.Windows.Forms.TreeView();
            this.codeEditor = new Xuld.JsonFormator.ExtendedRichTextBox();
            this.msMain.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpSource.SuspendLayout();
            this.cmEditor.SuspendLayout();
            this.tpObject.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPasteAndFormat,
            this.miFormat,
            this.miStringify,
            this.miCollapse,
            this.miCopy,
            this.miHelp});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.msMain.Size = new System.Drawing.Size(786, 28);
            this.msMain.TabIndex = 0;
            this.msMain.Text = "ms";
            // 
            // miPasteAndFormat
            // 
            this.miPasteAndFormat.Name = "miPasteAndFormat";
            this.miPasteAndFormat.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.miPasteAndFormat.Size = new System.Drawing.Size(133, 24);
            this.miPasteAndFormat.Text = "粘贴并格式化(&Q)";
            this.miPasteAndFormat.Click += new System.EventHandler(this.miPasteAndFormat_Click);
            // 
            // miFormat
            // 
            this.miFormat.Name = "miFormat";
            this.miFormat.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.miFormat.Size = new System.Drawing.Size(84, 24);
            this.miFormat.Text = "格式化(&F)";
            this.miFormat.Click += new System.EventHandler(this.miFormat_Click);
            // 
            // miStringify
            // 
            this.miStringify.Name = "miStringify";
            this.miStringify.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.miStringify.Size = new System.Drawing.Size(70, 24);
            this.miStringify.Text = "压缩(&S)";
            this.miStringify.Click += new System.EventHandler(this.miStringify_Click);
            // 
            // miCollapse
            // 
            this.miCollapse.Name = "miCollapse";
            this.miCollapse.Size = new System.Drawing.Size(72, 24);
            this.miCollapse.Text = "折叠(&D)";
            this.miCollapse.Click += new System.EventHandler(this.miCollapse_Click);
            // 
            // miCopy
            // 
            this.miCopy.Name = "miCopy";
            this.miCopy.Size = new System.Drawing.Size(71, 24);
            this.miCopy.Text = "复制(&C)";
            this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHomepage,
            this.miAbout});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(73, 24);
            this.miHelp.Text = "帮助(&H)";
            // 
            // miHomepage
            // 
            this.miHomepage.Name = "miHomepage";
            this.miHomepage.Size = new System.Drawing.Size(251, 24);
            this.miHomepage.Text = "打开产品主页...(&P)";
            this.miHomepage.Click += new System.EventHandler(this.miHomepage_Click);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(251, 24);
            this.miAbout.Text = "关于 JSON 格式化工具(&A)";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // cbUseStrictMode
            // 
            this.cbUseStrictMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUseStrictMode.AutoSize = true;
            settings1.cbStrictMode = false;
            settings1.SettingsKey = "";
            this.cbUseStrictMode.Checked = settings1.cbStrictMode;
            this.cbUseStrictMode.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "cbStrictMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbUseStrictMode.Location = new System.Drawing.Point(673, 3);
            this.cbUseStrictMode.Name = "cbUseStrictMode";
            this.cbUseStrictMode.Size = new System.Drawing.Size(113, 19);
            this.cbUseStrictMode.TabIndex = 2;
            this.cbUseStrictMode.Text = "严格模式(&S)";
            // 
            // tvImageList
            // 
            this.tvImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tvImageList.ImageStream")));
            this.tvImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.tvImageList.Images.SetKeyName(0, "field.bmp");
            this.tvImageList.Images.SetKeyName(1, "array.bmp");
            this.tvImageList.Images.SetKeyName(2, "obj.bmp");
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 555);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(786, 25);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(39, 20);
            this.lbStatus.Text = "就绪";
            // 
            // tcMain
            // 
            this.tcMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcMain.Controls.Add(this.tpSource);
            this.tcMain.Controls.Add(this.tpObject);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tcMain.Location = new System.Drawing.Point(0, 28);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(786, 527);
            this.tcMain.TabIndex = 5;
            // 
            // tpSource
            // 
            this.tpSource.Controls.Add(this.codeEditor);
            this.tpSource.Location = new System.Drawing.Point(4, 31);
            this.tpSource.Name = "tpSource";
            this.tpSource.Padding = new System.Windows.Forms.Padding(3);
            this.tpSource.Size = new System.Drawing.Size(778, 492);
            this.tpSource.TabIndex = 0;
            this.tpSource.Text = "源码视图";
            this.tpSource.UseVisualStyleBackColor = true;
            // 
            // cmEditor
            // 
            this.cmEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiFormat,
            this.tmiUndo,
            this.toolStripMenuItem1,
            this.tmiCut,
            this.tmiCopy,
            this.tmiPastle,
            this.tmiPastleAndFormat});
            this.cmEditor.Name = "cmEditor";
            this.cmEditor.Size = new System.Drawing.Size(191, 154);
            this.cmEditor.Opening += new System.ComponentModel.CancelEventHandler(this.cmEditor_Opening);
            // 
            // tmiFormat
            // 
            this.tmiFormat.Name = "tmiFormat";
            this.tmiFormat.Size = new System.Drawing.Size(190, 24);
            this.tmiFormat.Text = "格式化(&F)";
            this.tmiFormat.Click += new System.EventHandler(this.miFormat_Click);
            // 
            // tmiUndo
            // 
            this.tmiUndo.Name = "tmiUndo";
            this.tmiUndo.Size = new System.Drawing.Size(190, 24);
            this.tmiUndo.Text = "撤销(&U)";
            this.tmiUndo.Click += new System.EventHandler(this.miUndo_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(187, 6);
            // 
            // tmiCut
            // 
            this.tmiCut.Name = "tmiCut";
            this.tmiCut.Size = new System.Drawing.Size(190, 24);
            this.tmiCut.Text = "剪切(&T)";
            this.tmiCut.Click += new System.EventHandler(this.miCut_Click);
            // 
            // tmiCopy
            // 
            this.tmiCopy.Name = "tmiCopy";
            this.tmiCopy.Size = new System.Drawing.Size(190, 24);
            this.tmiCopy.Text = "复制(&C)";
            this.tmiCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // tmiPastle
            // 
            this.tmiPastle.Name = "tmiPastle";
            this.tmiPastle.Size = new System.Drawing.Size(190, 24);
            this.tmiPastle.Text = "粘贴(&P)";
            this.tmiPastle.Click += new System.EventHandler(this.miPastle_Click);
            // 
            // tmiPastleAndFormat
            // 
            this.tmiPastleAndFormat.Name = "tmiPastleAndFormat";
            this.tmiPastleAndFormat.Size = new System.Drawing.Size(190, 24);
            this.tmiPastleAndFormat.Text = "粘贴并格式化(&Q)";
            this.tmiPastleAndFormat.Click += new System.EventHandler(this.miPasteAndFormat_Click);
            // 
            // tpObject
            // 
            this.tpObject.Controls.Add(this.tvOjectTree);
            this.tpObject.Location = new System.Drawing.Point(4, 31);
            this.tpObject.Name = "tpObject";
            this.tpObject.Padding = new System.Windows.Forms.Padding(3);
            this.tpObject.Size = new System.Drawing.Size(778, 492);
            this.tpObject.TabIndex = 1;
            this.tpObject.Text = "对象视图";
            this.tpObject.UseVisualStyleBackColor = true;
            // 
            // tvOjectTree
            // 
            this.tvOjectTree.ContextMenuStrip = this.cmEditor;
            this.tvOjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvOjectTree.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvOjectTree.HideSelection = false;
            this.tvOjectTree.ImageIndex = 2;
            this.tvOjectTree.ImageList = this.tvImageList;
            this.tvOjectTree.Location = new System.Drawing.Point(3, 3);
            this.tvOjectTree.Name = "tvOjectTree";
            this.tvOjectTree.SelectedImageIndex = 2;
            this.tvOjectTree.ShowNodeToolTips = true;
            this.tvOjectTree.Size = new System.Drawing.Size(772, 486);
            this.tvOjectTree.TabIndex = 0;
            this.tvOjectTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvOjectTree_BeforeExpand);
            // 
            // codeEditor
            // 
            this.codeEditor.AcceptsTab = true;
            this.codeEditor.ContextMenuStrip = this.cmEditor;
            this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditor.EnableAutoDragDrop = true;
            this.codeEditor.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeEditor.Location = new System.Drawing.Point(3, 3);
            this.codeEditor.Name = "codeEditor";
            this.codeEditor.Size = new System.Drawing.Size(772, 486);
            this.codeEditor.TabIndex = 1;
            this.codeEditor.Text = "";
            this.codeEditor.TextChanged += new System.EventHandler(this.codeEditor_TextChanged);
            this.codeEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.codeEditor_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 580);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.cbUseStrictMode);
            this.Controls.Add(this.msMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.Name = "MainForm";
            this.Text = "JSON 格式化工具";
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tpSource.ResumeLayout(false);
            this.cmEditor.ResumeLayout(false);
            this.tpObject.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem miPasteAndFormat;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miFormat;
        private System.Windows.Forms.CheckBox cbUseStrictMode;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miHomepage;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem miCollapse;
        private System.Windows.Forms.ImageList tvImageList;
        private System.Windows.Forms.ToolStripMenuItem miStringify;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpSource;
        private System.Windows.Forms.TabPage tpObject;
        private System.Windows.Forms.TreeView tvOjectTree;
        private System.Windows.Forms.ContextMenuStrip cmEditor;
        private System.Windows.Forms.ToolStripMenuItem tmiPastleAndFormat;
        private System.Windows.Forms.ToolStripMenuItem tmiFormat;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tmiUndo;
        private System.Windows.Forms.ToolStripMenuItem tmiCut;
        private System.Windows.Forms.ToolStripMenuItem tmiPastle;
        private Xuld.JsonFormator.ExtendedRichTextBox codeEditor;
    }
}

