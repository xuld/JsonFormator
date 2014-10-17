using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xuld.Tetris;

namespace Xuld.JsonFormator {
    partial class AboutForm : Form {

        #region 设计器支持

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox logoPictureBox;
        private Label labelProductName;
        private Label labelCopyright;
        private TextBox textBoxDescription;
        private Button okButton;
        private Label labelCompanyName;
        private LinkLabel linkLabel1;

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.okButton, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(6, 3);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.33787F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.33787F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.33787F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.54649F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.43991F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(427, 227);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Location = new System.Drawing.Point(4, 3);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 5);
            this.logoPictureBox.Size = new System.Drawing.Size(132, 221);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(148, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 20);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(275, 20);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "产品名称";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.Location = new System.Drawing.Point(148, 25);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 20);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(275, 20);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "©2014 xuld 版权所有";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(148, 78);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(8, 3, 4, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.ShortcutsEnabled = false;
            this.textBoxDescription.Size = new System.Drawing.Size(275, 106);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "说明";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(323, 197);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 27);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "确定(&O)";
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.Location = new System.Drawing.Point(148, 50);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 20);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(275, 20);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "更多作品请访问：";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(270, 51);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(175, 15);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://work.xuld.net/";
            // 
            // AboutForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 233);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "关于";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region 窗口

        public AboutForm() {
            InitializeComponent();

            var assemby = Assembly.GetExecutingAssembly();

            Font = System.Drawing.SystemFonts.CaptionFont;

            if (Form.ActiveForm != null) {
                Icon = Form.ActiveForm.Icon;
                Text = "关于 " + Form.ActiveForm.Text;
            }

            labelProductName.Text = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assemby, typeof(AssemblyTitleAttribute))).Title + " " + assemby.GetName().Version.ToString();
            textBoxDescription.Text = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assemby, typeof(AssemblyDescriptionAttribute))).Description;

        }

        /// <summary>
        /// 显示新的关于窗口。
        /// </summary>
        /// <returns></returns>
        public static AboutForm show(IWin32Window owner) {
            var form = new AboutForm();
            form.ShowDialog(owner);
            return form;
        }

        /// <summary>
        /// 显示新的关于窗口。
        /// </summary>
        /// <returns></returns>
        public static AboutForm show() {
            var form = new AboutForm();
            form.ShowDialog();
            return form;
        }

        #endregion

        #region 游戏

        protected override void OnLoad(EventArgs e) {

            ActiveControl = null;

            int BorderWidth = (this.Width - this.ClientSize.Width) / 2;
            int TitlebarHeight = this.Height - this.ClientSize.Height - 2 * BorderWidth;

            game.GameOver += game_GameOver;
            game.Draw += game_Draw;

            logoPictureBox.Image = bitmap;

            timer.Tick += timer_Tick;

            game.start();

            timer.Start();

        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '5':
                    game.DropShape();
                    e.Handled = true;
                    break;
                case '7':
                    game.MoveShapeLeft();
                    e.Handled = true;
                    break;
                case '8':
                    game.RotateShape();
                    e.Handled = true;
                    break;
                case '9':
                    game.MoveShapeRight();
                    e.Handled = true;
                    break;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Down:
                case Keys.NumPad2:
                case Keys.S:
                    game.DropShape();
                    return false;
                case Keys.Left:
                case Keys.NumPad4:
                case Keys.A:
                    game.MoveShapeLeft();
                    return false;
                case Keys.Up:
                case Keys.NumPad8:
                case Keys.W:
                    game.RotateShape();
                    return false;
                case Keys.Right:
                case Keys.NumPad6:
                case Keys.D:
                    game.MoveShapeRight();
                    return false;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }

        const int CupWidth = 10, CupHeight = 20, RectSize = 20, LittleRectSize = RectSize / 8;

        private Game game = new Game(CupWidth, CupHeight);

        private Bitmap bitmap = new Bitmap(CupWidth * RectSize, CupHeight * RectSize, PixelFormat.Format32bppArgb);

        private Timer timer = new Timer();

        private void game_Draw(object sender, DrawEventArgs args) {
            DrawField(bitmap, args.Cup, args.Shape, args.ShapePoint);
            
            timer.Interval = 1000 - args.Level * 100;

            logoPictureBox.Invalidate();
        }

        private void game_GameOver(object sender, GameOverEventArgs args) {
            //timer.Enabled = false;

            //timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e) {
            game.NextTick();
        }

        private void DrawField(Image image, Cup cup, Shape shape, Point shapePoint) {
            using (Graphics g = Graphics.FromImage(image)) {
                g.Clear(Color.Black);

                for (int i = 0; i < cup.Width; i++) {
                    for (int j = 0; j < cup.Height; j++) {
                        if (cup[i, j] == Cell.Occupied) {
                            DrawCell(g, i, j, cup[i, j], Color.Green);
                        } else {
                            DrawCell(g, i, j, cup[i, j], Color.Gray);
                        }
                    }
                }

                if (shape != null) {
                    for (int i = 0; i < shape.Width; i++) {
                        for (int j = 0; j < shape.Height; j++) {
                            if (shape.Cells[i, j] == Cell.Occupied) {
                                Rectangle rect = new Rectangle((shapePoint.X + i) * RectSize, (shapePoint.Y + j) * RectSize, RectSize, RectSize);
                                g.FillRectangle(Brushes.Black, rect);
                                DrawCell(g, shapePoint.X + i, shapePoint.Y + j, Cell.Occupied, Color.Red);
                            }
                        }
                    }
                }
            }
        }

        private void DrawCell(Graphics g, int i, int j, Cell cell, Color color) {
            Rectangle rect;

            if (cell == Cell.Free) {
                rect = new Rectangle(i * RectSize + RectSize / 2 - LittleRectSize / 2, j * RectSize + RectSize / 2 - LittleRectSize / 2, LittleRectSize, LittleRectSize);
            } else {
                rect = new Rectangle(i * RectSize, j * RectSize, RectSize, RectSize);
            }

            using (Pen pen = new Pen(color, LittleRectSize)) {
                g.DrawRectangle(pen, rect);
            }
        }

        #endregion

    }

}

namespace Xuld.Tetris {

    public class Game {
        public Game(int cupWidth, int cupHeight) {
            _Cup = new Cup(cupWidth, cupHeight);
        }

        public void start() {
            _Cup.Clear();
            _Level = 0;
            _Score = 0;
            _DroppedLinesCount = 0;

            NewShape();

            OnDraw();
        }


        public void NextTick() {
            Point newShapePoint = new Point(_ShapePoint.X, _ShapePoint.Y + 1);

            if (_Cup.CanShapeBePlaced(newShapePoint, _Shape)) {
                _ShapePoint = newShapePoint;
                OnDraw();
            } else {
                _Score += 10;
                int droppedLinesCount = _Cup.MergeShape(_Shape, _ShapePoint);
                _Score += droppedLinesCount * 100;
                _DroppedLinesCount += droppedLinesCount;
                _Level = Math.Min(9, Math.Max(0, _DroppedLinesCount / 10));

                NewShape();

                if (!_Cup.CanShapeBePlaced(_ShapePoint, _Shape)) {
                    OnGameOver();
                    start();
                } else {
                    OnDraw();
                }
            }
        }

        public void DropShape() {
            Point point = _ShapePoint;
            Point prevPoint;

            do {
                prevPoint = point;
                point = new Point(prevPoint.X, prevPoint.Y + 1);
            } while (_Cup.CanShapeBePlaced(point, _Shape));

            _ShapePoint = prevPoint;

            OnDraw();
        }

        public void MoveShapeLeft() {
            Point point = new Point(_ShapePoint.X - 1, _ShapePoint.Y);

            if (_Cup.CanShapeBePlaced(point, _Shape)) {
                _ShapePoint = point;
                OnDraw();
            }
        }

        public void MoveShapeRight() {
            Point point = new Point(_ShapePoint.X + 1, _ShapePoint.Y);

            if (_Cup.CanShapeBePlaced(point, _Shape)) {
                _ShapePoint = point;
                OnDraw();
            }
        }

        public void RotateShape() {
            Shape newShape = _Shape.Rotate();
            int x = _ShapePoint.X + _Shape.Width / 2 - newShape.Width / 2;

            if (x < 0) {
                x = 0;
            } else if (x + newShape.Width >= _Cup.Width) {
                x = _Cup.Width - newShape.Width;
            }

            Point point = new Point(x, _ShapePoint.Y);

            if (_Cup.CanShapeBePlaced(point, newShape)) {
                _ShapePoint = point;
                _Shape = newShape;
                OnDraw();
            }
        }

        private void NewShape() {
            _Shape = RandomShape();
            _ShapePoint = new Point(_Cup.Width / 2 - _Shape.Width / 2, 0);
        }

        private Shape RandomShape() {
            return _Shapes[_Random.Next(_Shapes.Length)];
        }

        private event GameOverEventHandler _GameOver;
        public event GameOverEventHandler GameOver {
            add {
                _GameOver += value;
            }
            remove {
                _GameOver -= value;
            }
        }

        private void OnDraw() {
            if (_Draw != null) {
                _Draw(this, new DrawEventArgs(_Cup, _Shape, _ShapePoint, _Level, _Score));
            }
        }

        private void OnGameOver() {
            if (_GameOver != null) {
                _GameOver(this, new GameOverEventArgs(_Level, _Score));
            }
        }

        private event DrawEventHandler _Draw;
        public event DrawEventHandler Draw {
            add {
                _Draw += value;
            }
            remove {
                _Draw -= value;
            }
        }

        private Cup _Cup;
        private Shape _Shape;
        private Point _ShapePoint;
        private int _Level;
        private int _Score;
        private int _DroppedLinesCount;
        private Random _Random = new Random();

        private static readonly Shape[] _Shapes = new Shape[]
        {
            // ****
            new Shape(new Cell[,] { 
               { Cell.Occupied }, 
               { Cell.Occupied }, 
               { Cell.Occupied }, 
               { Cell.Occupied } 
            } ), 

            //  **
            // **
            new Shape(new Cell[,] { 
                { Cell.Occupied, Cell.Free }, 
                { Cell.Occupied, Cell.Occupied }, 
                { Cell.Free, Cell.Occupied} 
            } ), 

            // **
            //  **
            new Shape(new Cell[,] { 
                { Cell.Free, Cell.Occupied }, 
                { Cell.Occupied, Cell.Occupied }, 
                { Cell.Occupied, Cell.Free} 
            } ), 

            // **
            // **
            new Shape(new Cell[,] { 
                { Cell.Occupied, Cell.Occupied }, 
                { Cell.Occupied, Cell.Occupied } 
            } ),

            // ***
            // *
            new Shape(new Cell[,] { 
                { Cell.Occupied, Cell.Occupied },
                { Cell.Free, Cell.Occupied },
                { Cell.Free, Cell.Occupied }
            } ), 

            // ***
            //   *
            new Shape(new Cell[,] { 
                { Cell.Free, Cell.Occupied },
                { Cell.Free, Cell.Occupied },
                { Cell.Occupied, Cell.Occupied }
            } ), 

            // ***
            //  *
            new Shape(new Cell[,] { 
                { Cell.Free, Cell.Occupied },
                { Cell.Occupied, Cell.Occupied },
                { Cell.Free, Cell.Occupied }
            } )
        };
    }

    public enum Cell {
        Free,
        Occupied
    }

    public class Cup {
        public int Width {
            get {
                return _Cells.GetLength(0);
            }
        }

        public int Height {
            get {
                return _Cells.GetLength(1);
            }
        }

        public Cup(int width, int height) {
            _Cells = new Cell[width, height];
        }

        public Cell this[int row, int column] {
            get {
                return _Cells[row, column];
            }
        }

        public void Clear() {
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    _Cells[i, j] = Cell.Free;
                }
            }
        }

        public int MergeShape(Shape shape, Point shapePoint) {
            for (int i = 0; i < shape.Width; i++) {
                for (int j = 0; j < shape.Height; j++) {
                    if (shape.Cells[i, j] == Cell.Occupied &&
                        IsInternalPoint(shapePoint.X + i, shapePoint.Y + j)) {
                        _Cells[shapePoint.X + i, shapePoint.Y + j] = Cell.Occupied;
                    }
                }
            }

            int droppedLinesCount = 0;

            for (int j = _Cells.GetLength(1) - 1; j >= 0; j--) {
                bool flag = true;

                for (int i = 0; i < _Cells.GetLength(0); i++) {
                    if (_Cells[i, j] == Cell.Free) {
                        flag = false;
                        break;
                    }
                }

                if (flag) {
                    droppedLinesCount++;
                    DropLine(j++);
                }
            }

            return droppedLinesCount;
        }

        private void DropLine(int column) {
            for (int j = column; j > 0; j--) {
                for (int i = 0; i < Width; i++) {
                    _Cells[i, j] = _Cells[i, j - 1];
                }
            }
        }

        private bool IsInternalPoint(int x, int y) {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public bool CanShapeBePlaced(Point shapePoint, Shape shape) {
            if (shapePoint.X >= 0 && shapePoint.Y >= 0 &&
                shapePoint.X + shape.Width <= Width &&
                shapePoint.Y + shape.Height <= Height) {
                bool flag = true;

                for (int i = 0; i < shape.Width; i++) {
                    for (int j = 0; j < shape.Height; j++) {
                        if (shape.Cells[i, j] == Cell.Occupied &&
                            _Cells[shapePoint.X + i, shapePoint.Y + j] == Cell.Occupied) {
                            flag = false;
                            break;
                        }
                    }

                    if (!flag) {
                        break;
                    }
                }

                return flag;
            } else {
                return false;
            }
        }

        private Cell[,] _Cells;
    }

    public class DrawEventArgs : GameOverEventArgs {
        public Cup Cup {
            get;
            private set;
        }

        public Shape Shape {
            get;
            private set;
        }

        public Point ShapePoint {
            get;
            private set;
        }

        public DrawEventArgs(Cup cup, Shape shape, Point shapePoint, int level, int score)
            : base(level, score) {
            if (cup == null) {
                throw new ArgumentNullException("cup");
            }

            if (shape == null) {
                throw new ArgumentNullException("shape");
            }

            if (shapePoint == null) {
                throw new ArgumentNullException("shapePoint");
            }

            Cup = cup;
            Shape = shape;
            ShapePoint = shapePoint;
        }
    }

    public delegate void DrawEventHandler(object sender, DrawEventArgs eventArgs);

    public class GameOverEventArgs : EventArgs {
        public int Level {
            get;
            private set;
        }

        public int Score {
            get;
            private set;
        }

        public GameOverEventArgs(int level, int score)
            : base() {
            Level = level;
            Score = score;
        }
    }

    public delegate void GameOverEventHandler(object sender, GameOverEventArgs eventArgs);

    public class Shape {
        public Cell[,] Cells {
            get;
            private set;
        }

        public int Width {
            get {
                return Cells.GetLength(0);
            }
        }

        public int Height {
            get {
                return Cells.GetLength(1);
            }
        }

        public Shape(Cell[,] cells) {
            if (cells == null) {
                throw new ArgumentNullException("cells");
            }

            Cells = new Cell[cells.GetLength(0), cells.GetLength(1)];

            for (int i = 0; i < cells.GetLength(0); i++) {
                for (int j = 0; j < cells.GetLength(1); j++) {
                    Cells[i, j] = cells[i, j];
                }
            }
        }

        private Shape() {

        }

        public Shape Rotate() {
            Cell[,] cells = new Cell[Cells.GetLength(1), Cells.GetLength(0)];

            for (int i = 0; i < Cells.GetLength(0); i++) {
                for (int j = 0; j < Cells.GetLength(1); j++) {
                    cells[j, i] = Cells[i, Cells.GetLength(1) - 1 - j];
                }
            }

            Shape shape = new Shape();
            shape.Cells = cells;

            return shape;
        }
    }

}
