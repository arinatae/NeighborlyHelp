using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace NeighborlyHelp
{
    public class GameView : Form
    {
        private GameController _controller; // ← убрали readonly
        private readonly GameModel _model;

        public GameView(GameController controller, GameModel model)
        {
            _controller = controller;
            _model = model ?? throw new ArgumentNullException(nameof(model));

            InitializeComponent();
            SetupEvents();
        }

        public GameController Controller
        {
            set => _controller = value ?? throw new ArgumentNullException(nameof(value));
        }

        private void InitializeComponent()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                           ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.Text = "🏡 Соседская помощь ";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(0, 0);
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = ColorTranslator.FromHtml("#87CEEB ");
            this.KeyPreview = true;
        }

        private void SetupEvents()
        {
            this.MouseClick += OnMouseClick;
            this.MouseDown += OnMouseDown;
            this.MouseUp += OnMouseUp;
            this.MouseMove += OnMouseMove;
            this.KeyDown += OnKeyDown;
            this.Resize += OnResize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _controller.Render(e.Graphics);
        }

        private void OnMouseClick(object? sender, MouseEventArgs e) => _controller.HandleMouseClick(e);
        private void OnMouseDown(object? sender, MouseEventArgs e) => _controller.HandleMouseDown(e);
        private void OnMouseUp(object? sender, MouseEventArgs e) => _controller.HandleMouseUp(e);
        private void OnMouseMove(object? sender, MouseEventArgs e) => _controller.HandleMouseMove(e);
        private void OnKeyDown(object? sender, KeyEventArgs e) => _controller.HandleKeyDown(e);
        private void OnResize(object? sender, EventArgs e) => _controller.HandleResize();

        public void InvalidateView() => this.Invalidate();
        public void ShowMessage(string message, string title) => MessageBox.Show(message, title);
        public void ExitGame() => Application.Exit();
    }
}