using NeighborlyHelp.Models;
using NeighborlyHelp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace NeighborlyHelp
{
    public class GameController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly DialogueService _dialogueService;
        private readonly QuestService _questService;
        private Timer _gameTimer;

        public GameController(GameModel model, GameView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));

            // Инициализация сервисов
            _dialogueService = new DialogueService();
            _questService = new QuestService(_model);

            _gameTimer = new Timer { Interval = 16 };
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // Подписка на изменения модели для перерисовки (Паттерн Observer)
            _model.OnStateChanged += () => _view.InvalidateView();
        }

        public void StartGame()
        {
            // Генерация уровня через фабрику
            LevelFactory.GenerateLevel(_model);

            // Подписка на таймер подсказок
            if (_model.HintTimer != null)
            {
                _model.HintTimer.Tick += (s, e) =>
                {
                    _model.InteractionHint = "";
                    _view.InvalidateView();
                };
            }

            // Запуск сюжета через сервис
            _questService.StartStory();
            _view.InvalidateView();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (_model.IsFlowerGameActive && _model.IsWatering)
            {
                _model.WaterFlowers(_model.WateringPos);
                _model.CheckFlowerGameWin();
            }

            if (_model.IsRadioGameActive)
            {
                _model.CheckRadioGameWin();
            }

            _view.InvalidateView();
        }

        // === ПОЛНАЯ ЛОГИКА ОТРИСОВКИ (ИСПРАВЛЕНО: было пусто) ===
        public void Render(Graphics g)
        {
            // 1. Фон
            if (_model.BackgroundImage != null)
                g.DrawImage(_model.BackgroundImage, 0, 0, _model.GameField.Width, _model.GameField.Height);
            else
                g.Clear(_view.BackColor);

            // 2. Объекты
            foreach (var obj in _model.GameObjects)
                obj.Draw(g);

            // 3. Игрок
            if (_model.PlayerSprite != null)
                g.DrawImage(_model.PlayerSprite, _model.Player.X, _model.Player.Y, _model.Player.Width, _model.Player.Height);

            // 4. Подсказка взаимодействия
            if (!string.IsNullOrWhiteSpace(_model.InteractionHint))
            {
                Font hintFont = new Font("Arial", 14, FontStyle.Bold);
                SizeF hintSize = g.MeasureString(_model.InteractionHint, hintFont);
                float x = _model.Player.X + _model.Player.Width / 2 - hintSize.Width / 2;
                float y = _model.Player.Y - 30;

                using (Brush bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                    g.FillRectangle(bgBrush, x - 5, y - 2, hintSize.Width + 10, hintSize.Height + 4);

                g.DrawString(_model.InteractionHint, hintFont, Brushes.White, x, y);
            }

            // === МИНИ-ИГРА: ПОЛИВ ЦВЕТОВ ===
            if (_model.IsFlowerGameActive)
            {
                using (Brush overlay = new SolidBrush(Color.FromArgb(210, 10, 30, 10)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                g.DrawString("🌿 Полей все цветы из лейки", titleFont, Brushes.LightGreen,
                    new PointF((_view.ClientSize.Width - 380) / 2, 40));

                foreach (var f in _model.Flowers)
                {
                    if (_model.FlowerSprite != null)
                    {
                        int drawW = f.Bounds.Width - 20;
                        int drawH = f.Bounds.Height - 40;
                        int drawX = f.Bounds.X + 10;
                        int drawY = f.Bounds.Y + 10;
                        g.DrawImage(_model.FlowerSprite, drawX, drawY, drawW, drawH);
                    }
                    else
                    {
                        g.FillEllipse(Brushes.LimeGreen, f.Bounds.X + 10, f.Bounds.Y + 10, f.Bounds.Width - 20, f.Bounds.Height - 40);
                    }

                    float ratio = f.WaterLevel / 100f;
                    int barW = f.Bounds.Width - 20;
                    int barH = 8;
                    int barX = f.Bounds.X + 10;
                    int barY = f.Bounds.Y + f.Bounds.Height - 20;

                    g.FillRectangle(Brushes.Gray, barX, barY, barW, barH);
                    g.FillRectangle(Brushes.Cyan, barX, barY, barW * ratio, barH);
                    g.DrawRectangle(Pens.White, barX, barY, barW, barH);
                }

                if (_model.IsWatering)
                    g.DrawString("💧", new Font("Arial", 24), Brushes.White, _model.WateringPos.X - 12, _model.WateringPos.Y - 35);

                return;
            }

            // === МИНИ-ИГРА: РАДИО ===
            if (_model.IsRadioGameActive)
            {
                using (Brush overlay = new SolidBrush(Color.FromArgb(200, 20, 10, 30)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                g.DrawString("📻 Настрой радио на " + _model.TargetFreq.ToString("F1") + " МГц", titleFont, Brushes.LightYellow,
                    new PointF((_view.ClientSize.Width - 420) / 2, _model.RadioBarBounds.Y - 60));

                g.FillRectangle(Brushes.DarkGray, _model.RadioBarBounds);
                g.DrawRectangle(Pens.Silver, _model.RadioBarBounds);

                float targetRatio = (_model.TargetFreq - 88.0f) / 20.0f;
                int zoneX = _model.RadioBarBounds.X + (int)(_model.RadioBarBounds.Width * targetRatio);
                int zoneW = 30;
                g.FillRectangle(Brushes.LightGreen, zoneX - zoneW / 2, _model.RadioBarBounds.Y, zoneW, _model.RadioBarBounds.Height);

                float freqRatio = (_model.RadioFreq - 88.0f) / 20.0f;
                int needleX = _model.RadioBarBounds.X + (int)(_model.RadioBarBounds.Width * freqRatio);
                g.FillRectangle(Brushes.Red, needleX - 3, _model.RadioBarBounds.Y - 10, 6, _model.RadioBarBounds.Height + 20);

                Font freqFont = new Font("Arial", 16, FontStyle.Bold);
                g.DrawString(_model.RadioFreq.ToString("F1") + " MHz", freqFont, Brushes.White,
                    new PointF(needleX - 25, _model.RadioBarBounds.Y - 35));

                g.DrawString("Зажми ЛКМ и двигай мышь влево/вправо", new Font("Arial", 12), Brushes.Gray,
                    new PointF((_view.ClientSize.Width - 320) / 2, _model.RadioBarBounds.Bottom + 20));

                return;
            }

            // === МИНИ-ИГРА: ПОЧТА ===
            if (_model.IsMiniGameActive)
            {
                using (Brush overlay = new SolidBrush(Color.FromArgb(220, 30, 30, 40)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                Font hintFont = new Font("Arial", 20, FontStyle.Bold);
                string hintText = "Найди коробку с номером 18046";
                SizeF hintSize = g.MeasureString(hintText, hintFont);
                g.DrawString(hintText, hintFont, Brushes.Yellow,
                    new PointF((_view.ClientSize.Width - hintSize.Width) / 2, 30));

                Font boxFont = new Font("Arial", 11, FontStyle.Bold);
                foreach (var box in _model.MailOptions)
                {
                    if (_model.BoxSprite != null)
                        g.DrawImage(_model.BoxSprite, box.Bounds);
                    else
                    {
                        g.FillRectangle(Brushes.SaddleBrown, box.Bounds);
                        g.DrawRectangle(Pens.Gold, box.Bounds);
                    }

                    SizeF textSize = g.MeasureString(box.Number, boxFont);
                    PointF textPoint = new PointF(
                         box.Bounds.X + (box.Bounds.Width - textSize.Width) / 2,
                        box.Bounds.Y + (box.Bounds.Height - textSize.Height) / 2 + 25);
                    g.DrawString(box.Number, boxFont, Brushes.White, textPoint);
                }
                return;
            }

            // === ДИАЛОГОВОЕ ОКНО (ИСПРАВЛЕНО: теперь рисуется корректно) ===
            if (_model.IsDialogueActive)
            {
                using (Brush dimBrush = new SolidBrush(Color.FromArgb(180, 20, 20, 30)))
                    g.FillRectangle(dimBrush, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                int panelH = 200;
                int panelW = _view.ClientSize.Width - 120;
                int panelX = 60;
                int panelY = _view.ClientSize.Height - panelH - 40;

                bool isPlayerTurn = (_model.DialogueLineIndex % 2 != 0);
                string currentName = isPlayerTurn ? _model.PlayerDisplayName : _model.DialogueSpeaker;
                Bitmap? currentImg = isPlayerTurn ? _model.PlayerPortrait : _model.DialogueSprite;

                if (currentImg != null)
                {
                    int targetH = 800;
                    int targetW = (int)(targetH * ((float)currentImg.Width / currentImg.Height));
                    int spriteX = panelX + 50;
                    int spriteY = panelY - targetH + 10;
                    g.DrawImage(currentImg, spriteX, spriteY, targetW, targetH);
                }

                using (Brush panelBrush = new SolidBrush(Color.FromArgb(245, 235, 215)))
                using (Pen panelPen = new Pen(Color.FromArgb(120, 90, 60), 3))
                {
                    g.FillRectangle(panelBrush, panelX, panelY, panelW, panelH);
                    g.DrawRectangle(panelPen, panelX, panelY, panelW, panelH);
                }

                Font nameFont = new Font("Arial", 14, FontStyle.Bold);
                SizeF nameSize = g.MeasureString(currentName, nameFont);
                int nameW = (int)nameSize.Width + 30;
                int nameH = 28;
                int nameX = panelX + 25;
                int nameY = panelY - 14;

                using (Brush nameBgBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                using (Pen namePen = new Pen(Color.FromArgb(120, 90, 60), 2))
                {
                    g.FillRectangle(nameBgBrush, nameX, nameY, nameW, nameH);
                    g.DrawRectangle(namePen, nameX, nameY, nameW, nameH);
                }
                g.DrawString(currentName, nameFont, Brushes.Black, nameX + 15, nameY + 4);

                string currentText = "";
                if (_model.DialogueLines != null && _model.DialogueLineIndex >= 0 && _model.DialogueLineIndex < _model.DialogueLines.Count)
                    currentText = _model.DialogueLines[_model.DialogueLineIndex];

                Font textFont = new Font("Comic Sans", 23, FontStyle.Regular);
                RectangleF textRect = new RectangleF(panelX + 30, panelY + 25, panelW - 60, panelH - 40);
                using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near })
                    g.DrawString(currentText, textFont, Brushes.Black, textRect, sf);

                Font arrowFont = new Font("Comic Sans", 12, FontStyle.Bold);
                g.DrawString("▼ Нажми, чтобы продолжить", arrowFont, Brushes.Gray, panelX + panelW - 220, panelY + panelH - 30);

                return;
            }

            g.DrawString("Кликни на соседа для диалога",
                new Font("Arial", 9), Brushes.DarkGray, 10, 10);
        }

        // === ОБРАБОТКА ВВОДА ===
        public void HandleMouseClick(MouseEventArgs e)
        {
            if (_model.IsDialogueActive)
            {
                _model.AdvanceDialogue();
                _view.InvalidateView();
                _questService.HandleDialogueEnd();
                return;
            }

            if (_model.IsMiniGameActive)
            {
                foreach (var box in _model.MailOptions)
                {
                    if (box.Bounds.Contains(e.X, e.Y))
                    {
                        if (box.IsCorrect)
                        {
                            _view.ShowMessage("Посылка №18046 найдена! Отнеси её Оливеру.", "Успех");
                            _model.IsMiniGameActive = false;
                            _model.MailOptions.Clear();
                            _model.CurrentGameState = GameState.Quest2_Deliver;
                            _view.InvalidateView();
                        }
                        else
                        {
                            _view.ShowMessage("Не та коробка! Ищи посылку №18046.", "Ошибка");
                        }
                        return;
                    }
                }
                return;
            }

            if (_model.CurrentGameState == GameState.Quest2_Spawn)
            {
                foreach (var obj in _model.GameObjects)
                {
                    if (obj is Mailbox && obj.Bounds.Contains(e.X, e.Y))
                    {
                        _model.StartMailboxMiniGame(_view.ClientSize.Width, _view.ClientSize.Height);
                        _view.InvalidateView();
                        return;
                    }
                }
            }

            if (_model.CurrentGameState == GameState.Quest3_Spawn)
            {
                foreach (var obj in _model.GameObjects)
                {
                    if (obj is FlowerBed && obj.Bounds.Contains(e.X, e.Y))
                    {
                        _model.StartFlowerMiniGame(_view.ClientSize.Width, _view.ClientSize.Height);
                        _view.InvalidateView();
                        return;
                    }
                }
            }

            if (_model.CurrentGameState == GameState.Quest4_Talk)
            {
                foreach (var obj in _model.GameObjects)
                {
                    if (obj is Radio && obj.Bounds.Contains(e.X, e.Y))
                    {
                        _model.StartRadioMiniGame(_view.ClientSize.Width, _view.ClientSize.Height);
                        _view.InvalidateView();
                        return;
                    }
                }
            }

            foreach (var item in _model.Collectibles)
            {
                if (!item.IsPickedUp && item.Bounds.Contains(e.X, e.Y))
                {
                    if (!_model.IsCloseTo(item.Bounds))
                    {
                        _model.InteractionHint = "Подойдите ближе!";
                        _model.HintTimer.Stop(); _model.HintTimer.Start();
                        _view.InvalidateView();
                        return;
                    }

                    item.IsPickedUp = true;
                    _model.InteractionHint = "";

                    if (_model.CurrentGameState == GameState.Quest1_Find && item.Item.Name == "Ключи")
                    {
                        _view.ShowMessage("Нашёл ключи! Отнеси их Миле.", "Находка");
                        _model.CurrentGameState = GameState.Quest1_Return;
                    }
                    return;
                }
            }

            foreach (var npc in _model.NPCs)
            {
                if (npc.IsDialogAvailable && npc.Bounds.Contains(e.X, e.Y))
                {
                    if (!_model.IsCloseTo(npc.Bounds))
                    {
                        _model.InteractionHint = "Подойдите ближе!";
                        _model.HintTimer.Stop(); _model.HintTimer.Start();
                        _view.InvalidateView();
                        return;
                    }

                    StartDialogueWithNPC(npc);
                    return;
                }
            }
        }

        private void StartDialogueWithNPC(NPC npc)
        {
            var dialogueData = _dialogueService.GetDialogueFor(npc, _model.CurrentGameState);

            // Проверка на случай, если сервис вернул пустые данные
            if (dialogueData.NpcLines.Count == 0 && dialogueData.PlayerLines.Count == 0)
            {
                // Фоллбэк на старые данные NPC, если сервис не настроен
                StartDialogue(npc.DisplayName, npc.DialogLines, new List<string>(), "sprite1.png");
            }
            else
            {
                StartDialogue(npc.DisplayName, dialogueData.NpcLines, dialogueData.PlayerLines, dialogueData.SpriteName);
            }
        }

        private void StartDialogue(string speaker, List<string> npcLines, List<string> playerLines, string spriteFileName)
        {
            _model.IsDialogueActive = true;
            _model.DialogueSpeaker = speaker;
            _model.DialogueLineIndex = 0;

            var combined = new List<string>();
            for (int i = 0; i < npcLines.Count; i++)
            {
                combined.Add(npcLines[i]);
                if (i < playerLines.Count) combined.Add(playerLines[i]);
            }
            _model.DialogueLines = combined;

            NPC? n = _model.NPCs.Find(x => x.DisplayName == speaker);
            string pFile = n?.PortraitFileName ?? spriteFileName;
            try { _model.DialogueSprite = new Bitmap($"Assets/{pFile}"); }
            catch { try { _model.DialogueSprite = new Bitmap($"Assets/{spriteFileName}"); } catch { _model.DialogueSprite = null; } }

            _view.InvalidateView();
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            if (_model.IsFlowerGameActive)
            {
                _model.IsWatering = true;
                _model.WateringPos = e.Location;
            }

            if (_model.IsRadioGameActive && _model.RadioBarBounds.Contains(e.Location))
            {
                _model.IsDraggingRadio = true;
                _model.UpdateRadioFreq(e.X);
                _view.InvalidateView();
            }
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (_model.IsFlowerGameActive) _model.WateringPos = e.Location;

            if (_model.IsDraggingRadio)
            {
                _model.UpdateRadioFreq(e.X);
                _view.InvalidateView();
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            if (_model.IsFlowerGameActive) _model.IsWatering = false;
            if (_model.IsDraggingRadio) _model.IsDraggingRadio = false;
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            int newX = _model.Player.X;
            int newY = _model.Player.Y;
            int speed = _model.Player.Speed;

            switch (e.KeyCode)
            {
                case Keys.W: case Keys.Up: newY -= speed; break;
                case Keys.S: case Keys.Down: newY += speed; break;
                case Keys.A: case Keys.Left: newX -= speed; break;
                case Keys.D: case Keys.Right: newX += speed; break;

                case Keys.Escape:
                    _gameTimer.Stop();
                    var result = MessageBox.Show(
                         "Игра на паузе!\n\nВыберите действие:",
                         "Пауза",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes) _gameTimer.Start();
                    else if (result == DialogResult.No) _view.ExitGame();
                    else _gameTimer.Start();
                    return;
            }

            if (newX < 0 || newX > _model.GameField.Width - _model.Player.Width ||
                newY < 0 || newY > _model.GameField.Height - _model.Player.Height)
                return;

            Rectangle futureRect = new Rectangle(newX, newY, _model.Player.Width, _model.Player.Height);
            foreach (var obj in _model.GameObjects)
            {
                if (obj.IsSolid && futureRect.IntersectsWith(obj.Bounds))
                    return;
            }

            _model.MovePlayer(newX, newY);
            _view.InvalidateView();
        }

        public void HandleResize()
        {
            if (_model.GameField == null || _model.GameObjects == null) return;

            _model.GameField.Width = _view.ClientSize.Width;
            _model.GameField.Height = _view.ClientSize.Height;

            _model.GameObjects.RemoveAll(obj => obj is Wall);
            _model.GameObjects.Add(new Wall(0, 0, _model.GameField.Width, 10));
            _model.GameObjects.Add(new Wall(0, _model.GameField.Height - 10, _model.GameField.Width, 10));
            _model.GameObjects.Add(new Wall(0, 0, 10, _model.GameField.Height));
            _model.GameObjects.Add(new Wall(_model.GameField.Width - 10, 0, 10, _model.GameField.Height));

            _view.InvalidateView();
        }
    }
}