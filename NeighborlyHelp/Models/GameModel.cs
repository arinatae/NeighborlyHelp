using System;
using System.Collections.Generic;
using System.Drawing;
using NeighborlyHelp.Managers;
using NeighborlyHelp.Models;
using Timer = System.Windows.Forms.Timer;

namespace NeighborlyHelp
{
    // Перечисление всех возможных состояний игры для управления потоком квестов
    public enum GameState
    {
        Intro,
        Quest1_Talk,
        Quest1_Find,
        Quest1_Return,
        Quest2_Spawn,
        Quest2_MiniGame,
        Quest2_Deliver,
        Quest3_Spawn,
        Quest3_Talk,
        Quest3_Watering,
        Quest3_Completed,
        Quest4_Spawn,
        Quest4_Talk,
        Quest4_Radio,
        Quest4_Completed,
        Ending
    }

    public class GameModel
    {
        // Текущее состояние игрового процесса
        public GameState CurrentGameState { get; set; } = GameState.Intro;

        // Ссылка на объект игрока и игровое поле
        public Player Player { get; set; } = null!;
        public GameField GameField { get; set; } = null!;

        // Списки объектов на карте: декорации, препятствия и NPC
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        public List<NPC> NPCs { get; set; } = new List<NPC>();

        // Collectibles хранит подбираемые предметы (например, ключи), даже если полноценного инвентаря нет
        public List<Collectible> Collectibles { get; set; } = new List<Collectible>();

        // Менеджер для управления логикой квестов
        public QuestManager QuestManager { get; set; } = new QuestManager();

        // Флаги и данные для системы диалогов
        public bool IsDialogueActive { get; set; } = false;
        public string DialogueSpeaker { get; set; } = " ";
        public List<string> DialogueLines { get; set; } = new List<string>();
        public int DialogueLineIndex { get; set; } = 0;
        public Bitmap? DialogueSprite { get; set; } = null;
        public string PlayerDisplayName { get; set; } = "Ты ";
        public Bitmap? PlayerPortrait { get; set; } = null;

        // Флаги и данные для мини-игры с почтовым ящиком
        public bool IsMiniGameActive { get; set; } = false;
        public List<MailBoxOption> MailOptions { get; set; } = new List<MailBoxOption>();

        // Флаги и данные для мини-игры с поливом цветов
        public bool IsFlowerGameActive { get; set; } = false;
        public List<FlowerData> Flowers { get; set; } = new List<FlowerData>();
        public bool IsWatering { get; set; } = false;
        public Point WateringPos { get; set; } = Point.Empty;

        // Флаги и данные для мини-игры с радио
        public bool IsRadioGameActive { get; set; } = false;
        public float RadioFreq { get; set; } = 88.0f;
        public float TargetFreq { get; set; } = 95.5f;
        public bool IsDraggingRadio { get; set; } = false;
        public Rectangle RadioBarBounds { get; set; }

        // Подсказка взаимодействия, отображаемая near объектов
        public string InteractionHint { get; set; } = " ";
        public Timer HintTimer { get; set; } = null!;

        // Радиус, в пределах которого возможно взаимодействие с объектами
        public const int INTERACTION_RADIUS = 120;

        // Спрайты для отрисовки игрока, фона, ящиков и цветов
        public Bitmap? PlayerSprite { get; set; }
        public Bitmap? BackgroundImage { get; set; }
        public Bitmap? BoxSprite { get; set; }
        public Bitmap? FlowerSprite { get; set; }

        // Событие, уведомляющее View об изменении состояния модели для перерисовки
        public event Action OnStateChanged = delegate { };

        private void NotifyChanged()
        {
            OnStateChanged?.Invoke();
        }

        // Изображение для финальной заставки
        public Bitmap? EndingImage { get; set; } = null;

        public void Initialize()
        {
            // Инициализация игрового поля и создание объекта игрока с заданными параметрами
            GameField = new GameField();
            Player = new Player(530, 450)
            {
                Width = 200,
                Height = 200,
                Speed = 50
            };

            // Загрузка графических ресурсов из папки Assets с обработкой ошибок
            try { PlayerSprite = new Bitmap("Assets/sprite0.png"); } catch { }
            try { BackgroundImage = new Bitmap("Assets/backpicture.png"); } catch { }
            try { BoxSprite = new Bitmap("Assets/sprite-box.png"); } catch { }
            try { FlowerSprite = new Bitmap("Assets/spriteflower.png"); } catch { }
            try { PlayerPortrait = new Bitmap("Assets/portrait0.png"); } catch { }

            // Добавление статических объектов окружения (деревья, скамейки, клумбы, почтовый ящик)
            GameObjects.Add(new Tree(225, 15));
            GameObjects.Add(new Tree(800, 150));
            GameObjects.Add(new Tree(500, 800));
            GameObjects.Add(new Tree(1200, 730));
            GameObjects.Add(new Bench(800, 700));
            GameObjects.Add(new Bench(100, 330));
            GameObjects.Add(new FlowerBed(40, 450));
            GameObjects.Add(new Mailbox(1150, 45));

            // Создание границ карты (стены по периметру), чтобы игрок не выходил за пределы
            GameObjects.Add(new Wall(0, 0, GameField.Width, 10));
            GameObjects.Add(new Wall(0, GameField.Height - 10, GameField.Width, 10));
            GameObjects.Add(new Wall(0, 0, 10, GameField.Height));
            GameObjects.Add(new Wall(GameField.Width - 10, 0, 10, GameField.Height));

            // Настройка таймера для скрытия подсказок взаимодействия через 2 секунды
            HintTimer = new Timer { Interval = 2000 };
            HintTimer.Tick += (s, e) => { InteractionHint = " "; NotifyChanged(); };

            // Попытка загрузки изображения концовки по абсолютному пути для отладки
            string absolutePath = @"E:\ProjectsC#\NeighborlyHelp\NeighborlyHelp\NeighborlyHelp\Assets\end.png";

            try
            {
                if (System.IO.File.Exists(absolutePath))
                {
                    EndingImage = new Bitmap(absolutePath);
                    System.Diagnostics.Debug.WriteLine("!!! УСПЕХ: Картинка загружена по абсолютному пути !!!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"!!! ОШИБКА: Файл НЕ НАЙДЕН по пути: {absolutePath} !!!");
                    System.Diagnostics.Debug.WriteLine("Проверьте, лежит ли final.png именно в этой папке на диске.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! ИСКЛЮЧЕНИЕ ПРИ ЗАГРУЗКЕ: {ex.Message} !!!");
            }
        }

        // Проверка расстояния между игроком и целевым объектом для определения возможности взаимодействия
        public bool IsCloseTo(Rectangle targetBounds)
        {
            Rectangle playerRect = new Rectangle(Player.X, Player.Y, Player.Width, Player.Height);
            // Вычисление минимального расстояния между прямоугольниками
            int dx = Math.Max(0, Math.Max(playerRect.Left - targetBounds.Right, targetBounds.Left - playerRect.Right));
            int dy = Math.Max(0, Math.Max(playerRect.Top - targetBounds.Bottom, targetBounds.Top - playerRect.Bottom));
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= INTERACTION_RADIUS;
        }

        // Создание ключей на карте, если они еще не были подобраны
        public void SpawnKeys()
        {
            // Проверка, существуют ли уже ключи на карте и не подобраны ли они
            if (Collectibles.Exists(c => c.Item.Name == "Ключи" && !c.IsPickedUp)) return;

            Item keyItem = new Item("Ключи", "Блестящие ключи от домика", Color.Gold);
            Collectible keys = new Collectible(310, 460, keyItem, "spritekey.png");

            Collectibles.Add(keys);
            GameObjects.Add(keys);

            NotifyChanged();
        }

        // Создание нового NPC на карте с заданными параметрами
        public void SpawnNPC(string name, int x, int y, List<string> lines, string spriteName, int width, int height, string portraitFile = " ")
        {
            NPC newNpc = new NPC(x, y, name, lines, spriteName, width, height, portraitFile);
            NPCs.Add(newNpc);
            GameObjects.Add(newNpc);
            NotifyChanged();
        }

        // Удаление NPC с карты по имени
        public void RemoveNPC(string name)
        {
            var npc = NPCs.Find(n => n.DisplayName == name);
            if (npc != null)
            {
                GameObjects.Remove(npc);
                NPCs.Remove(npc);
                NotifyChanged();
            }
        }

        // Инициализация мини-игры с почтовым ящиком: генерация сетки номеров
        public void StartMailboxMiniGame(int clientWidth, int clientHeight)
        {
            MailOptions.Clear();
            Random rnd = new Random();
            int correctIndex = rnd.Next(0, 50); // Выбор случайного правильного номера

            int cols = 10, rows = 5, boxSize = 100, gap = 20;
            int totalWidth = cols * (boxSize + gap) - gap;
            int totalHeight = rows * (boxSize + gap) - gap;
            int startX = (clientWidth - totalWidth) / 2;
            int startY = (clientHeight - totalHeight) / 2 + 30;

            // Генерация 50 вариантов номеров, один из которых правильный
            for (int i = 0; i < 50; i++)
            {
                int row = i / cols, col = i % cols;
                int x = startX + col * (boxSize + gap);
                int y = startY + row * (boxSize + gap);
                string number = (i == correctIndex) ? "18046" : rnd.Next(10000, 99999).ToString();

                MailOptions.Add(new MailBoxOption
                {
                    Bounds = new Rectangle(x, y, boxSize, boxSize),
                    Number = number,
                    IsCorrect = (i == correctIndex)
                });
            }

            IsMiniGameActive = true;
            CurrentGameState = GameState.Quest2_MiniGame;
            NotifyChanged();
        }

        // Инициализация мини-игры с цветами: создание сетки клумб
        public void StartFlowerMiniGame(int clientWidth, int clientHeight)
        {
            Flowers.Clear();

            int cols = 5;
            int rows = 3;
            int cellSize = 100;

            int totalW = cols * cellSize;
            int totalH = rows * cellSize;

            int startX = (clientWidth - totalW) / 2;
            int startY = (clientHeight - totalH) / 2 - 50;

            // Создание 15 объектов цветов для полива
            for (int i = 0; i < 15; i++)
            {
                int r = i / cols;
                int c = i % cols;

                Flowers.Add(new FlowerData
                {
                    Bounds = new Rectangle(startX + c * cellSize, startY + r * cellSize, cellSize, cellSize)
                });
            }

            IsFlowerGameActive = true;
            CurrentGameState = GameState.Quest3_Watering;
            NotifyChanged();
        }

        // Инициализация мини-игры с радио: установка начальной и целевой частоты
        public void StartRadioMiniGame(int clientWidth, int clientHeight)
        {
            IsRadioGameActive = true;
            RadioFreq = 88.0f;
            // Целевая частота выбирается случайно в диапазоне 88-103 МГц
            TargetFreq = 88.0f + (float)(new Random().NextDouble() * 15);

            // Определение области отрисовки ползунка радио
            RadioBarBounds = new Rectangle(
                (clientWidth - 400) / 2,
                clientHeight / 2 - 20,
                400, 40
            );

            CurrentGameState = GameState.Quest4_Radio;
            NotifyChanged();
        }

        // Обновление частоты радио на основе позиции мыши относительно ползунка
        public void UpdateRadioFreq(int mouseX)
        {
            float ratio = (mouseX - RadioBarBounds.X) / (float)RadioBarBounds.Width;
            RadioFreq = 88.0f + ratio * 20.0f;
            // Ограничение частоты диапазоном FM-радио
            RadioFreq = Math.Max(88.0f, Math.Min(108.0f, RadioFreq));
        }

        // Переход к следующей реплике диалога или завершение диалога
        public void AdvanceDialogue()
        {
            DialogueLineIndex++;

            if (DialogueLineIndex >= DialogueLines.Count)
            {
                IsDialogueActive = false;
                DialogueSprite?.Dispose();
                DialogueSprite = null;
            }
            NotifyChanged();
        }

        // Перемещение игрока с проверкой коллизий со стенами и объектами
        public void MovePlayer(int newX, int newY)
        {
            int oldX = Player.X;
            int oldY = Player.Y;

            // Ограничение координат пределами игрового поля
            Player.X = Math.Max(0, Math.Min(newX, GameField.Width - Player.Width));
            Player.Y = Math.Max(0, Math.Min(newY, GameField.Height - Player.Height));

            Rectangle playerRect = new Rectangle(Player.X, Player.Y, Player.Width, Player.Height);

            // Проверка столкновений с твердыми объектами
            foreach (var obj in GameObjects)
            {
                if (obj.IsSolid && playerRect.IntersectsWith(obj.Bounds))
                {
                    // Откат позиции при столкновении
                    Player.X = oldX;
                    Player.Y = oldY;
                    break;
                }
            }
            NotifyChanged();
        }

        // Проверка условия победы в мини-игре с цветами (все цветы политы)
        public void CheckFlowerGameWin()
        {
            if (IsFlowerGameActive && Flowers.All(f => f.WaterLevel >= 100))
            {
                IsFlowerGameActive = false;
                IsWatering = false;
                CurrentGameState = GameState.Quest3_Completed;
                NotifyChanged();
            }
        }

        // Проверка условия победы в мини-игре с радио (частота совпадает с целевой)
        public void CheckRadioGameWin()
        {
            if (IsRadioGameActive && Math.Abs(RadioFreq - TargetFreq) <= 0.8f)
            {
                IsRadioGameActive = false;
                CurrentGameState = GameState.Quest4_Completed;
                NotifyChanged();
            }
        }

        // Логика полива цветов: увеличение уровня воды при клике по цветку
        public void WaterFlowers(Point pos)
        {
            if (!IsFlowerGameActive || !IsWatering) return;

            foreach (var f in Flowers)
            {
                if (!f.IsFull && f.Bounds.Contains(pos))
                {
                    f.WaterLevel += 4;
                    if (f.WaterLevel > 100) f.WaterLevel = 100;
                }
            }
        }
    }

    // Класс данных для отдельного цветка в мини-игре
    public class FlowerData
    {
        public Rectangle Bounds { get; set; }
        public int WaterLevel { get; set; } = 0;
        public bool IsFull => WaterLevel >= 100;
    }

    // Класс данных для варианта ответа в мини-игре с почтовым ящиком
    public class MailBoxOption
    {
        public Rectangle Bounds { get; set; }
        public string Number { get; set; } = " ";
        public bool IsCorrect { get; set; }
    }
}