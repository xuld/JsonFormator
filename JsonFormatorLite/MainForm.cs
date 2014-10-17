using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Circus.Json;
using System.Xml;
using System.Windows.Input;

namespace Xuld.JsonFormator {

    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private void formatOrStringifyJson(bool format) {

            // 更新状态栏。
            lbStatus.Text = "正在解析 JSON...";

            // 清空节点。
            tvOjectTree.SuspendLayout();
            tvOjectTree.Nodes.Clear();

            object jsonObject;
            try {
                jsonObject = Json.parse(codeEditor.Text, cbUseStrictMode.Checked);
            } catch (JsonParseException e) {

                tvOjectTree.ResumeLayout();

                // 更新状态栏。
                lbStatus.Text = "格式化错误：" + e.Message + "；行：" + e.startLine + "，列：" + e.startColumn;

                var start = codeEditor.GetFirstCharIndexFromLine(e.startLine - 1) + e.startColumn - 1;
                var end = codeEditor.GetFirstCharIndexFromLine(e.endLine - 1) + e.endColumn - 1;
                codeEditor.Select(start, end - start);
                codeEditor.ScrollToCaret();

                return ;
            }

            lbStatus.Text = "正在格式化...";

            // 显示格式化或转字符串后的源码。
            codeEditor.Text = Json.stringify(jsonObject, format);

            // 显示格式化后的对象树。
            TreeNode rootNode = createTreeNode("JSON", jsonObject);
            tvOjectTree.Nodes.Add(rootNode);

            // addTreeNode(tvOjectTree.Nodes, "JSON", jsonObject);

            // 默认展开 1 级。
            foreach (TreeNode node in tvOjectTree.Nodes) {
                node.Expand();
            }
            tvOjectTree.ResumeLayout();

            lbStatus.Text = "格式化成功";
            
        }

        private TreeNode createTreeNode(string label, object value) {
            var node = new TreeNode();

            string text = null;

            if (value is JsonObject) {
                node.ImageIndex = 2;
                node.Tag = value;

                if (((JsonObject) value).Count > 0) {
                    node.Nodes.Add(new TreeNode());
                }

            } else if (value is JsonArray) {
                node.ImageIndex = 1;
                node.Tag = value;

                if (((JsonArray)value).Count > 0) {
                    node.Nodes.Add(new TreeNode());
                }

            } else {
                node.ImageIndex = 0;
                text = Json.stringify(value);
            }

            if (text != null) {
                label = label == null ? text : (label + " : " + text);
            }

            node.ToolTipText = node.Text = label;
            node.SelectedImageIndex = node.ImageIndex;


            return node;

        }

        private void tvOjectTree_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            if (e.Node.Tag != null) {

                // 删除默认添加的空节点。
                tvOjectTree.SuspendLayout();

                e.Node.Nodes.Clear();

                if (e.Node.Tag is JsonObject) {
                    foreach (var vk in (JsonObject) e.Node.Tag) {
                        e.Node.Nodes.Add(createTreeNode(vk.Key, vk.Value));
                    }
                } else {
                    var i = 0;
                    foreach (var v in (JsonArray)e.Node.Tag) {
                        e.Node.Nodes.Add(createTreeNode(i++.ToString(),v));
                    }
                }
                e.Node.Tag = null;
                tvOjectTree.ResumeLayout();
            }
        }

        private static void addTreeNode(TreeNodeCollection parent, string label, object value) {

            var node = new TreeNode();
            parent.Add(node);
            node.Tag = value;

            string text = null;

            if (value is JsonObject) {
                node.ImageIndex = 2;

                foreach (var vk in (JsonObject)value) {
                    addTreeNode(node.Nodes, vk.Key, vk.Value);
                }

            } else if (value is JsonArray) {
                node.ImageIndex = 1;

                var i = 0;
                foreach (var v in (JsonArray)value) {
                    addTreeNode(node.Nodes, i++.ToString(), v);
                }

            } else {
                node.ImageIndex = 0;
                text = Json.stringify(value);
            }

            if (text != null) {
                label = label == null ? text : (label + " : " + text);
            }

            node.ToolTipText = node.Text = label;
            node.SelectedImageIndex = node.ImageIndex;


        }

        private void miPasteAndFormat_Click(object sender, EventArgs e) {
            codeEditor.Text = Clipboard.GetText();
            miFormat_Click(sender, e);
        }

        private void miFormat_Click(object sender, EventArgs e) {
            formatOrStringifyJson(true);
        }

        private void miStringify_Click(object sender, EventArgs e) {
            formatOrStringifyJson(false);
        }

        private void miCopy_Click(object sender, EventArgs e) {
            codeEditor.SelectAll();
            codeEditor.Copy();
        }

        private bool _collapse;

        private void miCollapse_Click(object sender, EventArgs e) {
            _collapse = !_collapse;
            if (_collapse) {
                tvOjectTree.CollapseAll();
            } else {
                tvOjectTree.ExpandAll();
            }

            if (tvOjectTree.Nodes.Count > 0) {
                tvOjectTree.Nodes[0].Expand();
            }

        }

        private void miHomepage_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://work.xuld.net/jsonformator");
        }

        private void miAbout_Click(object sender, EventArgs e) {
            AboutForm.show(this);
        }

        private void miUndo_Click(object sender, EventArgs e) {
            codeEditor.Undo();
        }

        private void miPastle_Click(object sender, EventArgs e) {
            codeEditor.Paste();
        }

        private void miDelete_Click(object sender, EventArgs e) {
            codeEditor.SelectedText = "";
        }

        private void miCut_Click(object sender, EventArgs e) {
            codeEditor.Cut();
        }

        private void cmEditor_Opening(object sender, CancelEventArgs e) {
            tmiUndo.Enabled = codeEditor.CanUndo;
        }

        private void codeEditor_KeyDown(object sender, KeyEventArgs e) {

            // 处理粘贴事件。
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                if (codeEditor.SelectionLength == codeEditor.TextLength) {
                    miPasteAndFormat_Click(sender, e);
                    codeEditor.Select(codeEditor.TextLength, 0);
                    e.Handled = true;
                }
            }
        }

        private void codeEditor_TextChanged(object sender, EventArgs e) {
            lbStatus.Text = "就绪";
        }

    }

}
