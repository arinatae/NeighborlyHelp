using NeighborlyHelp.Models;
using System.Collections.Generic; // Для List<T>
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer; // Явно указываем какой Timer использовать
using NeighborlyHelp.Managers;

namespace NeighborlyHelp
{
    public partial class Form1 : Form
    {
        private Player player = null!; // ! означает что мы гарантируем инициализацию
        private GameField gameField = null!;
        private Timer gameTimer = null!;
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<NPC> npcs = new List<NPC>();
        private Inventory inventory = new Inventory();
        private QuestManager questManager = new QuestManager();
        private List<Collectible> collectibles = new List<Collectible>();

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Фиксируем позицию окна
            this.StartPosition = FormStartPosition.CenterScreen;
            // Включаем двойную буферизацию (убирает мерцание)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.MouseClick += Form1_MouseClick;

            // Настройки окна
            this.Text = "🏡 Соседская помощь";
            this.Size = new Size(820, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#87CEEB");

            // Инициализация игровых объектов
            gameField = new GameField();
            player = new Player(400, 300); // Центр экрана

            // Создаём игровые объекты
            gameObjects.Add(new Tree(100, 100));
            gameObjects.Add(new Tree(650, 80));
            gameObjects.Add(new Bench(300, 450));
            gameObjects.Add(new Bench(500, 200));

            // Создаём соседей
            npcs.Add(new NPC(200, 200, "Бабушка Валя", new List<string>
            {
                "Ох, здравствуй, милок! 👋",
                "Не поможешь ли мне? Я ключи от подъезда потеряла...",
                "Кажется, уронила их где-то около скамейки. 🗝️"
            }));

            npcs.Add(new NPC(600, 400, "Дядя Петя", new List<string>
            {
                "Привет, сосед! 🛠️",
                "У меня тут посылка пришла, а забрать некогда — кран течёт!",
                "Не мог бы ты сходить за ней на почту? 📦"
            }));

            // Создаём предмет "Ключ" около скамейки
            Item keyItem = new Item("Ключи", "Блестящие ключи от подъезда", Color.Gold);
            Collectible keys = new Collectible(310, 460, keyItem); // Рядом со скамейкой (300, 450)
            collectibles.Add(keys);
            gameObjects.Add(keys); // Чтобы отрисовывался

            // Добавляем их в общий список объектов для отрисовки
            foreach (var npc in npcs)
            {
                gameObjects.Add(npc);
            }

            // Забор по периметру
            gameObjects.Add(new Wall(0, 0, 800, 10));           // Верх
            gameObjects.Add(new Wall(0, 590, 800, 10));         // Низ
            gameObjects.Add(new Wall(0, 0, 10, 600));           // Лево
            gameObjects.Add(new Wall(790, 0, 10, 600));         // Право

            // Игровой таймер (60 FPS)
            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            // Включаем обработку клавиш
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        // Обработка клика для диалога
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // 1. Сначала проверяем, кликнули ли по предмету
            foreach (var item in collectibles)
            {
                if (!item.IsPickedUp && item.Bounds.Contains(e.X, e.Y))
                {
                    // Подбираем!
                    item.IsPickedUp = true;
                    inventory.Add(item.Item);
                    MessageBox.Show($"✅ Вы подобрали: {item.Item.Name}!\n{item.Item.Description}",
                        "Инвентарь", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Если подобрали ключи - завершаем квест
                    if (item.Item.Name == "Ключи")
                    {
                        questManager.CompleteQuest("keys");
                    }
                    return; // Выходим, чтобы не открывать диалог
                }
            }

            // 2. Если не подобрали предмет, проверяем NPC (старый код)
            foreach (var npc in npcs)
            {
                if (npc.IsDialogAvailable && npc.Bounds.Contains(e.X, e.Y))
                {
                    DialogManager.ShowDialog(this, npc.DisplayName, npc.DialogLines);
                    return;
                }
            }
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            // Сохраняем старую позицию для отката при коллизии
            int oldX = player.X;
            int oldY = player.Y;

            // Ограничение границ поля
            player.X = Math.Max(0, Math.Min(player.X, gameField.Width - 20));
            player.Y = Math.Max(0, Math.Min(player.Y, gameField.Height - 20));

            // Проверка коллизий с объектами
            foreach (var obj in gameObjects)
            {
                if (obj.IsSolid)
                {
                    // Создаём прямоугольник игрока
                    Rectangle playerRect = new Rectangle(player.X, player.Y, 20, 20);

                    if (playerRect.IntersectsWith(obj.Bounds))
                    {
                        // Откатываем позицию если столкнулись с твёрдым объектом
                        player.X = oldX;
                        player.Y = oldY;
                        break;
                    }
                }
            }

            // Перерисовка
            this.Invalidate();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Управление WASD и стрелками
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    player.MoveUp();
                    break;
                case Keys.S:
                case Keys.Down:
                    player.MoveDown();
                    break;
                case Keys.A:
                case Keys.Left:
                    player.MoveLeft();
                    break;
                case Keys.D:
                case Keys.Right:
                    player.MoveRight();
                    break;
                case Keys.Escape:
                    gameTimer.Stop();
                    MessageBox.Show("Игра на паузе!\nНажми OK чтобы продолжить.",
                        "Пауза", MessageBoxButtons.OK, MessageBoxIcon.Information); // Исправлено Pause на Information
                    gameTimer.Start();
                    break;
                case Keys.I:
                    MessageBox.Show($"🎒 Инвентарь:\n{inventory.GetList()}", "Инвентарь",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        // Отрисовка игрока
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем все игровые объекты
            foreach (var obj in gameObjects)
            {
                obj.Draw(e.Graphics);
            }

            // Рисуем игрока (поверх всего)
            Brush playerBrush = new SolidBrush(Color.FromArgb(0, 120, 215));
            e.Graphics.FillRectangle(playerBrush, player.X, player.Y, 20, 20);

            Pen playerPen = new Pen(Color.White, 2);
            e.Graphics.DrawRectangle(playerPen, player.X, player.Y, 20, 20);

            // Отрисовка подсказки
            e.Graphics.DrawString("💡 Кликни на соседа с ! для диалога",
                new Font("Arial", 9), Brushes.DarkGray, 10, 10);
        }
    }
}