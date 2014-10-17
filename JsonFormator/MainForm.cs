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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using ICSharpCode.NRefactory;
using System.Xml;
using System.Windows.Input;

namespace Xuld.JsonFormator {
    public partial class MainForm : Form {

        private ICSharpCode.AvalonEdit.TextEditor codeEditor;

        private FoldingManager foldingManager;

        BraceFoldingStrategy _str = new BraceFoldingStrategy();

        public MainForm() {
            InitializeComponent();

            // 初始化编辑器。
            codeEditor = new ICSharpCode.AvalonEdit.TextEditor();
            codeEditorHost.Child = codeEditor;

            codeEditor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            codeEditor.FontSize = 14;
            codeEditor.ShowLineNumbers = true;
            codeEditor.SyntaxHighlighting = syntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".js");
            codeEditor.WordWrap = true;

            foldingManager = FoldingManager.Install(codeEditor.TextArea);
            codeEditor.TextChanged += codeEditor_TextChanged;

            //codeEditor.TextArea.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler(codeEditor_Paste));

            textMarkerService = new TextMarkerService(codeEditor);
            TextView textView = codeEditor.TextArea.TextView;
            textView.BackgroundRenderers.Add(textMarkerService);
            textView.LineTransformers.Add(textMarkerService);
            textView.Services.AddService(typeof(TextMarkerService), textMarkerService);
            textView.MouseHover += MouseHover;
            textView.MouseHoverStopped += codeEditorMouseHoverStopped;

            textView.MouseHover += MouseHover;
            textView.MouseHoverStopped += codeEditorMouseHoverStopped;
            textView.VisualLinesChanged += VisualLinesChanged;

        }

        private IHighlightingDefinition syntaxHighlighting;



        private new void MouseHover(object sender, System.Windows.Input.MouseEventArgs e) {
            var pos = codeEditor.TextArea.TextView.GetPositionFloor(e.GetPosition(codeEditor.TextArea.TextView) + codeEditor.TextArea.TextView.ScrollOffset);
            bool inDocument = pos.HasValue;
            if (inDocument) {
                TextLocation logicalPosition = pos.Value.Location;
                int offset = codeEditor.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                TextMarkerService.TextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null) {
                    if (toolTip == null) {
                        toolTip = new System.Windows.Controls.ToolTip();
                        toolTip.Closed += ToolTipClosed;
                        // toolTip.PlacementTarget = this;
                        toolTip.Content = new System.Windows.Controls.TextBlock {
                            Text = markerWithToolTip.ToolTip,
                            TextWrapping = System.Windows.TextWrapping.Wrap
                        };
                        toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        private readonly TextMarkerService textMarkerService;
        private System.Windows.Controls.ToolTip toolTip;

        void ToolTipClosed(object sender, System.Windows.RoutedEventArgs e) {
            toolTip = null;
        }

        void codeEditorMouseHoverStopped(object sender, System.Windows.Input.MouseEventArgs e) {
            if (toolTip != null) {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void VisualLinesChanged(object sender, EventArgs e) {
            if (toolTip != null) {
                toolTip.IsOpen = false;
            }
        }

        private void DisplayValidationError(string message, int startOffset, int endOffset) {
            if (endOffset == startOffset) {
                endOffset = TextUtilities.GetNextCaretPosition(codeEditor.Document, startOffset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
            }
            if (endOffset < 0) {
                endOffset = codeEditor.Document.TextLength;
            }
            int length = endOffset - startOffset;

            if (length < 2) {
                length = Math.Min(2, codeEditor.Document.TextLength - startOffset);
            }

            textMarkerService.Create(startOffset, length, message);
        }

        private void ClearError() {
            textMarkerService.Clear();
        }


        void codeEditor_TextChanged(object sender, EventArgs e) {

            if (codeEditor.Document.LineCount == 1 && codeEditor.Document.Lines[0].Length > 4096) {
                codeEditor.SyntaxHighlighting = null;
            } else {
                codeEditor.SyntaxHighlighting = syntaxHighlighting;
            }

            updateFolding();
            ClearError();
        }

        void updateFolding() {
            int firstErrorOffset;
            foldingManager.UpdateFoldings(_str.CreateNewFoldings(codeEditor.Document, out firstErrorOffset), firstErrorOffset);
        }

        /// <summary>
        /// 解析当前最新的 JSON 对象。
        /// </summary>
        /// <returns></returns>
        private object parseCurrentObject() {

            var json = codeEditor.Text;
            if (json.Length == 0) {
                return null;
            }

            try {
                return Json.parse(json, cbUseStrictMode.Checked);
            } catch (JsonParseException e) {
                lbStatus.Text = "格式化错误：" + e.Message + "；行：" + e.startLine + "，列：" + e.startColumn;

                var start = codeEditor.Document.GetOffset(e.startLine, e.startColumn);
                var end = codeEditor.Document.GetOffset(e.endLine, e.endColumn);
                codeEditor.Select(start, end - start);

                codeEditor.ScrollTo(e.endLine, e.endColumn);

                DisplayValidationError(e.Message, start, end);

                return null;
            }

        }

        private void miPasteAndFormat_Click(object sender, EventArgs e) {
            codeEditor.Clear();
            codeEditor.Paste();
            miFormat_Click(sender, e);
        }

        private void miFormat_Click(object sender, EventArgs e) {

            tvOjectTree.Nodes.Clear();

            var obj = parseCurrentObject();

            if (obj == null) {
                return;
            }

            // 显示格式化后的源码。
            string text = Json.stringify(obj, true);
            codeEditor.Document.Text = text;

            // 显示格式化后的对象树。
            addTreeNode(tvOjectTree.Nodes, "JSON", obj);

            // 默认展开 1 级。
            foreach (TreeNode node in tvOjectTree.Nodes) {
                node.Expand();
            }

            lbStatus.Text = "格式化成功";

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

        private void miStringify_Click(object sender, EventArgs e) {

            var obj = parseCurrentObject();

            if (obj == null) {
                return;
            }

            codeEditor.Text = Json.stringify(obj);

        }

        private void miCopy_Click(object sender, EventArgs e) {
            codeEditor.SelectAll();
            codeEditor.Copy();
        }

        private bool _collapse;

        private void miCollapse_Click(object sender, EventArgs e) {
            _collapse = !_collapse;
            foreach (var folding in foldingManager.AllFoldings) {
                folding.IsFolded = _collapse;
            }

            var folde = foldingManager.GetNextFolding(0);
            if (folde != null) {
                folde.IsFolded = false;
            }

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
            //MessageBox.Show(this, "JSON 格式化工具 v2.0\r\nby xuld @2014-9-24\r\n更多作品请访问：http://work.xuld.net/", Text);
        }

        private string _lastText;

        private void MainForm_Activated(object sender, EventArgs e) {

            var text = Clipboard.GetText();
            if (text.Length > 0 && text.StartsWith("{") && text.EndsWith("}") && _lastText != text) {
                _lastText = text;
                lbStatus.Text = "已自动粘贴，正在格式化...";
                miPasteAndFormat_Click(sender, e);
                lbStatus.Text = "已自动粘贴并格式化";
            }

        }

        private void MainForm_Deactivate(object sender, EventArgs e) {
            lbStatus.Text = "就绪";
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

    }

    public class TextMarkerService : IBackgroundRenderer, IVisualLineTransformer {
        private readonly TextEditor codeEditor;
        private readonly TextSegmentCollection<TextMarker> markers;

        public sealed class TextMarker : TextSegment {
            public TextMarker(int startOffset, int length) {
                StartOffset = startOffset;
                Length = length;
            }

            public System.Windows.Media.Color? BackgroundColor { get; set; }
            public System.Windows.Media.Color MarkerColor { get; set; }
            public string ToolTip { get; set; }
        }

        public TextMarkerService(TextEditor codeEditor) {
            this.codeEditor = codeEditor;
            markers = new TextSegmentCollection<TextMarker>(codeEditor.Document);
        }

        public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext) {
            if (markers == null || !textView.VisualLinesValid) {
                return;
            }
            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0) {
                return;
            }
            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (TextMarker marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
                if (marker.BackgroundColor != null) {
                    var geoBuilder = new BackgroundGeometryBuilder { AlignToWholePixels = true, CornerRadius = 3 };
                    geoBuilder.AddSegment(textView, marker);
                    System.Windows.Media.Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null) {
                        System.Windows.Media.Color color = marker.BackgroundColor.Value;
                        var brush = new System.Windows.Media.SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }
                foreach (System.Windows.Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker)) {
                    var startPoint = r.BottomLeft;
                    var endPoint = r.BottomRight;

                    var usedPen = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(marker.MarkerColor), 1);
                    usedPen.Freeze();
                    const double offset = 2.5;

                    int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                    var geometry = new System.Windows.Media.StreamGeometry();

                    using (System.Windows.Media.StreamGeometryContext ctx = geometry.Open()) {
                        ctx.BeginFigure(startPoint, false, false);
                        ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                    }

                    geometry.Freeze();

                    drawingContext.DrawGeometry(System.Windows.Media.Brushes.Transparent, usedPen, geometry);
                    break;
                }
            }
        }

        public KnownLayer Layer {
            get { return KnownLayer.Selection; }
        }

        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements) { }

        private IEnumerable<System.Windows.Point> CreatePoints(System.Windows.Point start, System.Windows.Point end, double offset, int count) {
            for (int i = 0; i < count; i++) {
                yield return new System.Windows.Point(start.X + (i * offset), start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }

        public void Clear() {
            foreach (TextMarker m in markers) {
                Remove(m);
            }
        }

        private void Remove(TextMarker marker) {
            if (markers.Remove(marker)) {
                Redraw(marker);
            }
        }

        private void Redraw(ISegment segment) {
            codeEditor.TextArea.TextView.Redraw(segment);
        }

        public void Create(int offset, int length, string message) {
            var m = new TextMarker(offset, length);
            markers.Add(m);
            m.MarkerColor = System.Windows.Media.Colors.Red;
            m.ToolTip = message;
            Redraw(m);
        }

        public IEnumerable<TextMarker> GetMarkersAtOffset(int offset) {
            return markers == null ? Enumerable.Empty<TextMarker>() : markers.FindSegmentsContaining(offset);
        }
    }

    /// <summary>
    /// Allows producing foldings from a document based on braces.
    /// </summary>
    public class BraceFoldingStrategy {

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset) {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document) {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            for (int i = 0; i < document.TextLength; i++) {
                char c = document.GetCharAt(i);
                if (c == '{' || c == '[') {
                    startOffsets.Push(i);
                } else if ((c == '}' || c == ']') && startOffsets.Count > 0) {
                    int startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset) {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                } else if (c == '\n' || c == '\r') {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }

}
