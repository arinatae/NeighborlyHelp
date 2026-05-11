using System;
using System.Collections.Generic;
using System.Drawing;
using NeighborlyHelp.Managers;
using NeighborlyHelp.Models;
using Timer = System.Windows.Forms.Timer;

namespace NeighborlyHelp
{
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
        Quest4_Completed
    }

    public class GameModel
    {
        public GameState CurrentGameState { get; set; } = GameState.Intro;

        public Player Player { get; set; } = null!;
        public GameField GameField { get; set; } = null!;

        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        public List<NPC> NPCs { get; set; } = new List<NPC>();

        // Вернули Collectibles, так как они нужны для ключей на карте
        public List<Collectible> Collectibles { get; set; } = new List<Collectible>();

        // Инвентарь удален по запросу. 
        // ВАЖНО: В GameController.cs нужно удалить обращения к _model.Inventory

        public QuestManager QuestManager { get; set; } = new QuestManager();

        public bool IsDialogueActive { get; set; } = false;
        public string DialogueSpeaker { get; set; } = ""; // Исправлено: была строка с пробелами "  "
        public List<string> DialogueLines { get; set; } = new List<string>();
        public int DialogueLineIndex { get; set; } = 0;
        public Bitmap? DialogueSprite { get; set; } = null;
        public string PlayerDisplayName { get; set; } = "Ты"; // Исправлено: был лишний пробел
        public Bitmap? PlayerPortrait { get; set; } = null;

        public bool IsMiniGameActive { get; set; } = false;
        public List<MailBoxOption> MailOptions { get; set; } = new List<MailBoxOption>();

        public bool IsFlowerGameActive { get; set; } = false;
        public List<FlowerData> Flowers { get; set; } = new List<FlowerData>();
        public bool IsWatering { get; set; } = false;
        public Point WateringPos { get; set; } = Point.Empty;

        public bool IsRadioGameActive { get; set; } = false;
        public float RadioFreq { get; set; } = 88.0f;
        public float TargetFreq { get; set; } = 95.5f;
        public bool IsDraggingRadio { get; set; } = false;
        public Rectangle RadioBarBounds { get; set; }

        public string InteractionHint { get; set; } = ""; // Исправлено: была строка с пробелами "  "
        public Timer HintTimer { get; set; } = null!;

        public const int INTERACTION_RADIUS = 120;

        public Bitmap? PlayerSprite { get; set; }
        public Bitmap? BackgroundImage { get; set; }
        public Bitmap? BoxSprite { get; set; }
        public Bitmap? FlowerSprite { get; set; }

        // Событие для паттерна Observer
        public event Action OnStateChanged = delegate { };

        private void NotifyChanged()
        {
            OnStateChanged?.Invoke();
        }

        public void Initialize()
        {
            GameField = new GameField();
            Player = new Player(530, 450)
            {
                Width = 200,
                Height = 200
            };

            try { PlayerSprite = new Bitmap("Assets/sprite0.png"); } catch { }
            try { BackgroundImage = new Bitmap("Assets/backpicture.png"); } catch { }
            try { BoxSprite = new Bitmap("Assets/sprite-box.png"); } catch { }
            try { FlowerSprite = new Bitmap("Assets/spriteflower.png"); } catch { }
            try { PlayerPortrait = new Bitmap("Assets/portrait0.png"); } catch { }

            GameObjects.Add(new Tree(225, 15));
            GameObjects.Add(new Tree(800, 150));
            GameObjects.Add(new Tree(500, 800));
            GameObjects.Add(new Tree(1200, 730));
            GameObjects.Add(new Bench(800, 700));
            GameObjects.Add(new Bench(100, 330));
            GameObjects.Add(new FlowerBed(40, 450));
            GameObjects.Add(new Mailbox(1150, 45));
            GameObjects.Add(new Wall(0, 0, GameField.Width, 10));
            GameObjects.Add(new Wall(0, GameField.Height - 10, GameField.Width, 10));
            GameObjects.Add(new Wall(0, 0, 10, GameField.Height));
            GameObjects.Add(new Wall(GameField.Width - 10, 0, 10, GameField.Height));

            HintTimer = new Timer { Interval = 2000 };
            // Исправлено: очищаем подсказку в пустую строку
            HintTimer.Tick += (s, e) => { InteractionHint = ""; NotifyChanged(); };
        }

        public bool IsCloseTo(Rectangle targetBounds)
        {
            Rectangle playerRect = new Rectangle(Player.X, Player.Y, Player.Width, Player.Height);
            int dx = Math.Max(0, Math.Max(playerRect.Left - targetBounds.Right, targetBounds.Left - playerRect.Right));
            int dy = Math.Max(0, Math.Max(playerRect.Top - targetBounds.Bottom, targetBounds.Top - playerRect.Bottom));
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= INTERACTION_RADIUS;
        }

        public void SpawnKeys()
        {
            // Проверка использует Collectibles, поэтому поле должно существовать
            if (Collectibles.Exists(c => c.Item.Name == "Ключи" && !c.IsPickedUp)) return;

            Item keyItem = new Item("Ключи", "Блестящие ключи от домика", Color.Gold);
            Collectible keys = new Collectible(310, 460, keyItem, "spritekey.png");

            Collectibles.Add(keys);
            GameObjects.Add(keys);

            NotifyChanged();
        }

        public void SpawnNPC(string name, int x, int y, List<string> lines, string spriteName, int width, int height, string portraitFile = "")
        {
            NPC newNpc = new NPC(x, y, name, lines, spriteName, width, height, portraitFile);
            NPCs.Add(newNpc);
            GameObjects.Add(newNpc);
            NotifyChanged();
        }

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

        public void StartMailboxMiniGame(int clientWidth, int clientHeight)
        {
            MailOptions.Clear();
            Random rnd = new Random();
            int correctIndex = rnd.Next(0, 50);

            int cols = 10, rows = 5, boxSize = 100, gap = 20;
            int totalWidth = cols * (boxSize + gap) - gap;
            int totalHeight = rows * (boxSize + gap) - gap;
            int startX = (clientWidth - totalWidth) / 2;
            int startY = (clientHeight - totalHeight) / 2 + 30;

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

        public void StartRadioMiniGame(int clientWidth, int clientHeight)
        {
            IsRadioGameActive = true;
            RadioFreq = 88.0f;
            TargetFreq = 88.0f + (float)(new Random().NextDouble() * 15);

            RadioBarBounds = new Rectangle(
                (clientWidth - 400) / 2,
                clientHeight / 2 - 20,
                400, 40
            );

            CurrentGameState = GameState.Quest4_Radio;
            NotifyChanged();
        }

        public void UpdateRadioFreq(int mouseX)
        {
            float ratio = (mouseX - RadioBarBounds.X) / (float)RadioBarBounds.Width;
            RadioFreq = 88.0f + ratio * 20.0f;
            RadioFreq = Math.Max(88.0f, Math.Min(108.0f, RadioFreq));
        }

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

        public void MovePlayer(int newX, int newY)
        {
            int oldX = Player.X;
            int oldY = Player.Y;

            Player.X = Math.Max(0, Math.Min(newX, GameField.Width - Player.Width));
            Player.Y = Math.Max(0, Math.Min(newY, GameField.Height - Player.Height));

            Rectangle playerRect = new Rectangle(Player.X, Player.Y, Player.Width, Player.Height);
            foreach (var obj in GameObjects)
            {
                if (obj.IsSolid && playerRect.IntersectsWith(obj.Bounds))
                {
                    Player.X = oldX;
                    Player.Y = oldY;
                    break;
                }
            }
            NotifyChanged();
        }

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

        public void CheckRadioGameWin()
        {
            if (IsRadioGameActive && Math.Abs(RadioFreq - TargetFreq) <= 0.8f)
            {
                IsRadioGameActive = false;
                CurrentGameState = GameState.Quest4_Completed;
                NotifyChanged();
            }
        }

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

    public class FlowerData
    {
        public Rectangle Bounds { get; set; }
        public int WaterLevel { get; set; } = 0;
        public bool IsFull => WaterLevel >= 100;
    }

    public class MailBoxOption
    {
        public Rectangle Bounds { get; set; }
        public string Number { get; set; } = ""; // Исправлено: был пробел " "
        public bool IsCorrect { get; set; }
    }
}