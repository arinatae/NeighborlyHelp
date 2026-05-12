using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeighborlyHelp.Views
{
    // Класс представления (View), отвечающий за отрисовку интерфейса и обработку ввода пользователя
    public class GameView : Form
    {
        private GameController _controller;
        private readonly GameModel _model;

        // Конструктор формы: принимает контроллер и модель, инициализирует компоненты
        public GameView(GameController controller, GameModel model)
        {
            _controller = controller;
            _model = model ?? throw new ArgumentNullException(nameof(model));

            InitializeComponent();
            SetupEvents();
        }

        // Свойство для установки или замены контроллера после создания формы
        public GameController Controller
        {
            set => _controller = value ?? throw new ArgumentNullException(nameof(value));
        }

        // Первоначальная настройка свойств формы: размер, стиль отрисовки, заголовок
        private void InitializeComponent()
        {
            // Включение двойной буферизации для предотвращения мерцания при перерисовке
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            this.Text = "🏡 Соседская помощь ";
            // Убираем стандартную рамку окна для полноэкранного режима игры
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;

            // Установка размера формы на весь экран
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(0, 0);
            this.StartPosition = FormStartPosition.Manual;

            // Цвет фона по умолчанию (небесно-голубой)
            this.BackColor = ColorTranslator.FromHtml("#87CEEB ");

            // Позволяет форме перехватывать нажатия клавиш до передачи их элементам управления
            this.KeyPreview = true;
        }

        // Подписка событий ввода (мышь, клавиатура) и изменения размера на методы-обработчики
        private void SetupEvents()
        {
            this.MouseClick += OnMouseClick;
            this.MouseDown += OnMouseDown;
            this.MouseUp += OnMouseUp;
            this.MouseMove += OnMouseMove;
            this.KeyDown += OnKeyDown;
            this.Resize += OnResize;
        }

        // Переопределение метода отрисовки: передает объект Graphics контроллеру для рендеринга игры
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_controller != null)
            {
                _controller.Render(e.Graphics);
            }
        }

        // Обработчик клика мыши: передает событие контроллеру
        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            _controller?.HandleMouseClick(e);
        }

        // Обработчик нажатия кнопки мыши: передает событие контроллеру
        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            _controller?.HandleMouseDown(e);
        }

        // Обработчик отпускания кнопки мыши: передает событие контроллеру
        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            _controller?.HandleMouseUp(e);
        }

        // Обработчик движения мыши: передает событие контроллеру
        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            _controller?.HandleMouseMove(e);
        }

        // Обработчик нажатия клавиши клавиатуры: передает событие контроллеру
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _controller?.HandleKeyDown(e);
        }

        // Обработчик изменения размера окна: уведомляет контроллер для адаптации интерфейса
        private void OnResize(object? sender, EventArgs e)
        {
            _controller?.HandleResize();
        }

        // Метод для принудительной перерисовки формы (вызывается из модели/контроллера при изменении состояния)
        public void InvalidateView()
        {
            this.Invalidate();
        }

        // Отображение стандартного системного сообщения (MessageBox)
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        // Завершение работы приложения
        public void ExitGame()
        {
            Application.Exit();
        }
    }
}