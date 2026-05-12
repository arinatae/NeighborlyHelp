using NeighborlyHelp.Models;
using NeighborlyHelp.Services;
using NeighborlyHelp.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace NeighborlyHelp
{
    /// <summary>
    /// GameController управляет основной логикой игры, связывая модель (данные) и представление (отрисовку).
    /// Он обрабатывает ввод пользователя, обновляет состояние игры и запускает мини-игры.
    /// </summary>
    public class GameController
    {
        private readonly GameModel _model; // Модель данных игры
        private readonly GameView _view;   // Представление (форма/панель отрисовки)
        private readonly DialogueService _dialogueService; // Сервис для управления диалогами
        private readonly QuestService _questService;       // Сервис для управления квестами и сюжетом
        private Timer _gameTimer;          // Таймер основного игрового цикла

        /// <summary>
        /// Конструктор контроллера. Инициализирует сервисы, запускает игровой таймер
        /// и подписывается на события изменения состояния модели для перерисовки экрана.
        /// </summary>
        public GameController(GameModel model, GameView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));

            // Инициализация сервисов
            _dialogueService = new DialogueService();
            _questService = new QuestService(_model);

            // Настройка таймера игрового цикла (60 FPS, интервал ~16мс)
            _gameTimer = new Timer { Interval = 16 };
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // Подписка на изменения модели: при изменении данных вызывается перерисовка视图 (Паттерн Observer)
            _model.OnStateChanged += () => _view.InvalidateView();
        }

        /// <summary>
        /// Запуск игры: генерация уровня, запуск сюжета и инициализация таймера подсказок.
        /// </summary>
        public void StartGame()
        {
            // Генерация начального уровня через фабрику
            LevelFactory.GenerateLevel(_model);

            // Подписка на таймер подсказок взаимодействия (скрытие подсказки через время)
            if (_model.HintTimer != null)
            {
                _model.HintTimer.Tick += (s, e) =>
                {
                    _model.InteractionHint = " ";
                    _view.InvalidateView();
                };
            }

            // Запуск начальной сюжетной линии
            _questService.StartStory();
            _view.InvalidateView();
        }

        /// <summary>
        /// Основной игровой цикл. Выполняется каждые ~16мс.
        /// Обновляет логику активных мини-игр и запрашивает перерисовку экрана.
        /// </summary>
        private void GameLoop(object? sender, EventArgs e)
        {
            // Логика мини-игры "Полив цветов"
            if (_model.IsFlowerGameActive && _model.IsWatering)
            {
                _model.WaterFlowers(_model.WateringPos);
                _model.CheckFlowerGameWin();
            }

            // Логика мини-игры "Радио"
            if (_model.IsRadioGameActive)
            {
                _model.CheckRadioGameWin();
            }

            // Запрос на перерисовку экрана
            _view.InvalidateView();
        }

        /// <summary>
        /// Метод отрисовки всего содержимого игры.
        /// Рисует фон, объекты, игрока, интерфейсы мини-игр и диалоговые окна.
        /// </summary>
        public void Render(Graphics g)
        {
            // === 1. Отрисовка фона ===
            if (_model.BackgroundImage != null)
                g.DrawImage(_model.BackgroundImage, 0, 0, _model.GameField.Width, _model.GameField.Height);
            else
                g.Clear(_view.BackColor);

            // === 2. Отрисовка объектов окружения (стены, мебель и т.д.) ===
            foreach (var obj in _model.GameObjects)
                obj.Draw(g);

            // === 3. Отрисовка игрока ===
            if (_model.PlayerSprite != null)
                g.DrawImage(_model.PlayerSprite, _model.Player.X, _model.Player.Y, _model.Player.Width, _model.Player.Height);

            // === 4. Отрисовка подсказки взаимодействия над игроком ===
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
                // Затемнение фона
                using (Brush overlay = new SolidBrush(Color.FromArgb(210, 10, 30, 10)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                // Заголовок мини-игры
                Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                g.DrawString("🌿 Полей все цветы из лейки", titleFont, Brushes.LightGreen,
                    new PointF((_view.ClientSize.Width - 380) / 2, 40));

                // Отрисовка цветов и прогресс-баров полива
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
                        // fallback: простой зеленый椭圆, если спрайт не загружен
                        g.FillEllipse(Brushes.LimeGreen, f.Bounds.X + 10, f.Bounds.Y + 10, f.Bounds.Width - 20, f.Bounds.Height - 40);
                    }

                    // Progress bar уровня воды
                    float ratio = f.WaterLevel / 100f;
                    int barW = f.Bounds.Width - 20;
                    int barH = 8;
                    int barX = f.Bounds.X + 10;
                    int barY = f.Bounds.Y + f.Bounds.Height - 20;

                    g.FillRectangle(Brushes.Gray, barX, barY, barW, barH); // Фон бара
                    g.FillRectangle(Brushes.Cyan, barX, barY, barW * ratio, barH); // Заполнение
                    g.DrawRectangle(Pens.White, barX, barY, barW, barH); // Рамка
                }

                // Курсор лейки
                if (_model.IsWatering)
                    g.DrawString("💧", new Font("Arial", 24), Brushes.White, _model.WateringPos.X - 12, _model.WateringPos.Y - 35);

                return; // Прерываем отрисовку остального мира
            }

            // === МИНИ-ИГРА: РАДИО ===
            if (_model.IsRadioGameActive)
            {
                // Затемнение фона
                using (Brush overlay = new SolidBrush(Color.FromArgb(200, 20, 10, 30)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                // Заголовок с целевой частотой
                Font titleFont = new Font("Arial", 20, FontStyle.Bold);
                g.DrawString("📻 Настрой радио на " + _model.TargetFreq.ToString("F1") + " МГц", titleFont, Brushes.LightYellow,
                    new PointF((_view.ClientSize.Width - 420) / 2, _model.RadioBarBounds.Y - 60));

                // Отрисовка шкалы частот
                g.FillRectangle(Brushes.DarkGray, _model.RadioBarBounds);
                g.DrawRectangle(Pens.Silver, _model.RadioBarBounds);

                // Зеленая зона целевой частоды
                float targetRatio = (_model.TargetFreq - 88.0f) / 20.0f;
                int zoneX = _model.RadioBarBounds.X + (int)(_model.RadioBarBounds.Width * targetRatio);
                int zoneW = 30;
                g.FillRectangle(Brushes.LightGreen, zoneX - zoneW / 2, _model.RadioBarBounds.Y, zoneW, _model.RadioBarBounds.Height);

                // Красная стрелка текущей частоты
                float freqRatio = (_model.RadioFreq - 88.0f) / 20.0f;
                int needleX = _model.RadioBarBounds.X + (int)(_model.RadioBarBounds.Width * freqRatio);
                g.FillRectangle(Brushes.Red, needleX - 3, _model.RadioBarBounds.Y - 10, 6, _model.RadioBarBounds.Height + 20);

                // Текст текущей частоты
                Font freqFont = new Font("Arial", 16, FontStyle.Bold);
                g.DrawString(_model.RadioFreq.ToString("F1") + " MHz", freqFont, Brushes.White,
                    new PointF(needleX - 25, _model.RadioBarBounds.Y - 35));

                // Подсказка управления
                g.DrawString("Зажми ЛКМ и двигай мышь влево/вправо", new Font("Arial", 12), Brushes.Gray,
                    new PointF((_view.ClientSize.Width - 320) / 2, _model.RadioBarBounds.Bottom + 20));

                return;
            }

            // === МИНИ-ИГРА: ПОЧТА (Поиск посылки) ===
            if (_model.IsMiniGameActive)
            {
                // Затемнение фона
                using (Brush overlay = new SolidBrush(Color.FromArgb(220, 30, 30, 40)))
                    g.FillRectangle(overlay, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                // Заголовок задания
                Font hintFont = new Font("Arial", 20, FontStyle.Bold);
                string hintText = "Найди коробку с номером 18046";
                SizeF hintSize = g.MeasureString(hintText, hintFont);
                g.DrawString(hintText, hintFont, Brushes.Yellow,
                    new PointF((_view.ClientSize.Width - hintSize.Width) / 2, 30));

                // Отрисовка коробок
                Font boxFont = new Font("Arial", 11, FontStyle.Bold);
                foreach (var box in _model.MailOptions)
                {
                    if (_model.BoxSprite != null)
                        g.DrawImage(_model.BoxSprite, box.Bounds);
                    else
                    {
                        // fallback: коричневый прямоугольник
                        g.FillRectangle(Brushes.SaddleBrown, box.Bounds);
                        g.DrawRectangle(Pens.Gold, box.Bounds);
                    }

                    // Номер на коробке
                    SizeF textSize = g.MeasureString(box.Number, boxFont);
                    PointF textPoint = new PointF(
                         box.Bounds.X + (box.Bounds.Width - textSize.Width) / 2,
                        box.Bounds.Y + (box.Bounds.Height - textSize.Height) / 2 + 25);
                    g.DrawString(box.Number, boxFont, Brushes.White, textPoint);
                }
                return;
            }

            // === ДИАЛОГОВОЕ ОКНО ===
            if (_model.IsDialogueActive)
            {
                // Затемнение фона
                using (Brush dimBrush = new SolidBrush(Color.FromArgb(180, 20, 20, 30)))
                    g.FillRectangle(dimBrush, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);

                // Параметры панели диалога
                int panelH = 200;
                int panelW = _view.ClientSize.Width - 120;
                int panelX = 60;
                int panelY = _view.ClientSize.Height - panelH - 40;

                // Определение говорящего (Игрок или NPC)
                bool isPlayerTurn = (_model.DialogueLineIndex % 2 != 0);
                string currentName = isPlayerTurn ? _model.PlayerDisplayName : _model.DialogueSpeaker;
                Bitmap? currentImg = isPlayerTurn ? _model.PlayerPortrait : _model.DialogueSprite;

                // Отрисовка портрета персонажа
                if (currentImg != null)
                {
                    int targetH = 800; // Высота портрета
                    int targetW = (int)(targetH * ((float)currentImg.Width / currentImg.Height));
                    int spriteX = panelX + 50;
                    int spriteY = panelY - targetH + 10;
                    g.DrawImage(currentImg, spriteX, spriteY, targetW, targetH);
                }

                // Отрисовка фона панели текста
                using (Brush panelBrush = new SolidBrush(Color.FromArgb(245, 235, 215)))
                using (Pen panelPen = new Pen(Color.FromArgb(120, 90, 60), 3))
                {
                    g.FillRectangle(panelBrush, panelX, panelY, panelW, panelH);
                    g.DrawRectangle(panelPen, panelX, panelY, panelW, panelH);
                }

                // Отрисовка имени говорящего
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

                // Отрисовка текста реплики
                string currentText = " ";
                if (_model.DialogueLines != null && _model.DialogueLineIndex >= 0 && _model.DialogueLineIndex < _model.DialogueLines.Count)
                    currentText = _model.DialogueLines[_model.DialogueLineIndex];

                Font textFont = new Font("Comic Sans", 23, FontStyle.Regular);
                RectangleF textRect = new RectangleF(panelX + 30, panelY + 25, panelW - 60, panelH - 40);
                using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near })
                    g.DrawString(currentText, textFont, Brushes.Black, textRect, sf);

                // Подсказка "Нажми, чтобы продолжить"
                Font arrowFont = new Font("Comic Sans", 12, FontStyle.Bold);
                g.DrawString("▼ Нажми, чтобы продолжить", arrowFont, Brushes.Gray, panelX + panelW - 220, panelY + panelH - 30);

                return;
            }

            // === ЭКРАН КОНЦОВКИ ===
            if (_model.CurrentGameState == GameState.Ending)
            {
                g.Clear(Color.White);

                // Если картинка концовки загружена, рисуем её на весь экран
                if (_model.EndingImage != null)
                {
                    try
                    {
                        g.DrawImage(_model.EndingImage, 0, 0, _view.ClientSize.Width, _view.ClientSize.Height);
                    }
                    catch
                    {
                        // Игнорируем ошибки отрисовки изображения
                    }
                }

                return;
            }

            // Подсказка в обычном режиме игры
            g.DrawString("Кликни на соседа для диалога",
                new Font("Arial", 9), Brushes.DarkGray, 10, 10);
        }

        /// <summary>
        /// Обработка клика левой кнопкой мыши.
        /// Управляет взаимодействием с объектами, NPC и элементами мини-игр.
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            // Если активен диалог, переходим к следующей реплике
            if (_model.IsDialogueActive)
            {
                _model.AdvanceDialogue();
                _view.InvalidateView();
                _questService.HandleDialogueEnd();
                return;
            }

            // Логика мини-игры "Почта": выбор коробки
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

            // Начало мини-игры "Почта" при клике на почтовый ящик
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

            // Начало мини-игры "Полив" при клике на клумбу
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

            // Начало мини-игры "Радио" при клике на радио
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

            // Подбор предметов (коллекционных элементов)
            foreach (var item in _model.Collectibles)
            {
                if (!item.IsPickedUp && item.Bounds.Contains(e.X, e.Y))
                {
                    // Проверка дистанции до предмета
                    if (!_model.IsCloseTo(item.Bounds))
                    {
                        _model.InteractionHint = "Подойдите ближе!";
                        _model.HintTimer.Stop();
                        _model.HintTimer.Start();
                        _view.InvalidateView();
                        return;
                    }

                    item.IsPickedUp = true;
                    _model.InteractionHint = " ";

                    // Специфическая логика для квеста с ключами
                    if (_model.CurrentGameState == GameState.Quest1_Find && item.Item.Name == "Ключи")
                    {
                        _view.ShowMessage("Нашёл ключи! Отнеси их Миле.", "Находка");
                        _model.CurrentGameState = GameState.Quest1_Return;
                    }
                    return;
                }
            }

            // Взаимодействие с NPC (начало диалога)
            foreach (var npc in _model.NPCs)
            {
                if (npc.IsDialogAvailable && npc.Bounds.Contains(e.X, e.Y))
                {
                    // Блокировка диалогов после завершения игры
                    if (_model.CurrentGameState == GameState.Ending)
                    {
                        return;
                    }

                    // Проверка дистанции до NPC
                    if (!_model.IsCloseTo(npc.Bounds))
                    {
                        _model.InteractionHint = "Подойдите ближе!";
                        if (_model.HintTimer != null)
                        {
                            _model.HintTimer.Stop();
                            _model.HintTimer.Start();
                        }
                        _view.InvalidateView();
                        return;
                    }

                    StartDialogueWithNPC(npc);
                    return;
                }
            }
        }

        /// <summary>
        /// Инициализирует диалог с выбранным NPC, загружая данные и спрайты.
        /// </summary>
        private void StartDialogueWithNPC(NPC npc)
        {
            var dialogueData = _dialogueService.GetDialogueFor(npc, _model.CurrentGameState);
            StartDialogue(npc.DisplayName, dialogueData.NpcLines, dialogueData.PlayerLines, dialogueData.SpriteName);
        }

        /// <summary>
        /// Запускает процесс диалога: формирует список реплик, загружает портрет собеседника.
        /// </summary>
        private void StartDialogue(string speaker, List<string> npcLines, List<string> playerLines, string spriteFileName)
        {
            _model.IsDialogueActive = true;
            _model.DialogueSpeaker = speaker;
            _model.DialogueLineIndex = 0;

            // Объединение реплик NPC и игрока в одну очередь
            var combined = new List<string>();
            for (int i = 0; i < npcLines.Count; i++)
            {
                combined.Add(npcLines[i]);
                if (i < playerLines.Count) combined.Add(playerLines[i]);
            }
            _model.DialogueLines = combined;

            // Загрузка портрета NPC
            NPC? n = _model.NPCs.Find(x => x.DisplayName == speaker);
            string pFile = n?.PortraitFileName ?? spriteFileName;

            // Попытка загрузки изображения с обработкой ошибок
            try { _model.DialogueSprite = new Bitmap($"Assets/{pFile}"); }
            catch { try { _model.DialogueSprite = new Bitmap($"Assets/{spriteFileName}"); } catch { _model.DialogueSprite = null; } }

            _view.InvalidateView();
        }

        /// <summary>
        /// Обработка нажатия кнопки мыши (MouseDown).
        /// Используется для начала действий в мини-играх (полив, настройка радио).
        /// </summary>
        public void HandleMouseDown(MouseEventArgs e)
        {
            // Начало полива в мини-игре цветов
            if (_model.IsFlowerGameActive)
            {
                _model.IsWatering = true;
                _model.WateringPos = e.Location;
            }

            // Начало перетаскивания ползунка в мини-игре радио
            if (_model.IsRadioGameActive && _model.RadioBarBounds.Contains(e.Location))
            {
                _model.IsDraggingRadio = true;
                _model.UpdateRadioFreq(e.X);
                _view.InvalidateView();
            }
        }

        /// <summary>
        /// Обработка движения мыши (MouseMove).
        /// Обновляет позицию лейки или частоту радио в зависимости от активной мини-игры.
        /// </summary>
        public void HandleMouseMove(MouseEventArgs e)
        {
            if (_model.IsFlowerGameActive) _model.WateringPos = e.Location;

            if (_model.IsDraggingRadio)
            {
                _model.UpdateRadioFreq(e.X);
                _view.InvalidateView();
            }
        }

        /// <summary>
        /// Обработка отпускания кнопки мыши (MouseUp).
        /// Прекращает действия в мини-играх.
        /// </summary>
        public void HandleMouseUp(MouseEventArgs e)
        {
            if (_model.IsFlowerGameActive) _model.IsWatering = false;
            if (_model.IsDraggingRadio) _model.IsDraggingRadio = false;
        }

        /// <summary>
        /// Обработка нажатий клавиш клавиатуры.
        /// Управление движением игрока, пауза и чит-коды разработчика.
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            // === ГОРЯЧИЕ КЛАВИШИ РАЗРАБОТЧИКА (Ctrl + цифра) ===
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1: // Ctrl+1: Пропустить поиск ключей (Квест 1)
                        _model.CurrentGameState = GameState.Quest1_Return;
                        _view.ShowMessage("Dev: Ключи найдены (Квест 1 пропущен)", "Debug");
                        return;

                    case Keys.D2: // Ctrl+2: Пропустить почту (Квест 2)
                        _model.CurrentGameState = GameState.Quest2_Deliver;
                        _view.ShowMessage("Dev: Посылка получена (Квест 2 пропущен)", "Debug");
                        return;

                    case Keys.D3: // Ctrl+3: Пропустить полив цветов (Квест 3)
                        _model.CurrentGameState = GameState.Quest3_Completed;
                        _view.ShowMessage("Dev: Цветы политы (Квест 3 пропущен)", "Debug");
                        return;

                    case Keys.D4: // Ctrl+4: Пропустить радио (Квест 4)
                        _model.CurrentGameState = GameState.Quest4_Completed;
                        _view.ShowMessage("Dev: Радио настроено (Квест 4 пропущен)", "Debug");
                        return;

                    case Keys.D5: // Ctrl+5: Сразу включить концовку
                        _model.CurrentGameState = GameState.Ending;
                        _view.ShowMessage("Dev: Включена концовка", "Debug");
                        return;

                    case Keys.R: // Ctrl+R: Рестарт игры
                        _model.Initialize();
                        _questService.StartStory();
                        _view.ShowMessage("Dev: Игра перезапущена", "Debug");
                        return;
                }
            }
            // ========================================

            // Расчет новых координат игрока
            int newX = _model.Player.X;
            int newY = _model.Player.Y;
            int speed = _model.Player.Speed;

            // Базовая скорость из модели
            int baseSpeed = _model.Player.Speed;

            int Speed = baseSpeed * 10;

            // Управление движением (WASD или стрелки)
            switch (e.KeyCode)
            {
                case Keys.W: case Keys.Up: newY -= speed; break;
                case Keys.S: case Keys.Down: newY += speed; break;
                case Keys.A: case Keys.Left: newX -= speed; break;
                case Keys.D: case Keys.Right: newX += speed; break;

                case Keys.Escape: // Пауза и выход
                    _gameTimer.Stop();
                    var result = MessageBox.Show(
                          "Игра на паузе!\n\nВыберите действие: ",
                          "Пауза",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes) _gameTimer.Start(); // Продолжить
                    else if (result == DialogResult.No) _view.ExitGame(); // Выйти
                    else _gameTimer.Start(); // Отмена (продолжить)
                    return;
            }

            // Проверка границ игрового поля
            if (newX < 0 || newX > _model.GameField.Width - _model.Player.Width ||
                newY < 0 || newY > _model.GameField.Height - _model.Player.Height)
                return;

            // Проверка столкновений со стенами и объектами
            Rectangle futureRect = new Rectangle(newX, newY, _model.Player.Width, _model.Player.Height);
            foreach (var obj in _model.GameObjects)
            {
                if (obj.IsSolid && futureRect.IntersectsWith(obj.Bounds))
                    return; // Движение заблокировано
            }

            // Применение нового положения игрока
            _model.MovePlayer(newX, newY);
            _view.InvalidateView();
        }

        /// <summary>
        /// Обработка изменения размера окна игры.
        /// Пересчитывает границы поля и пересоздает граничные стены.
        /// </summary>
        public void HandleResize()
        {
            if (_model.GameField == null || _model.GameObjects == null) return;

            // Обновление размеров игрового поля под размер клиента
            _model.GameField.Width = _view.ClientSize.Width;
            _model.GameField.Height = _view.ClientSize.Height;

            // Удаление старых граничных стен
            _model.GameObjects.RemoveAll(obj => obj is Wall);

            // Создание новых граничных стен по периметру
            _model.GameObjects.Add(new Wall(0, 0, _model.GameField.Width, 10)); // Верх
            _model.GameObjects.Add(new Wall(0, _model.GameField.Height - 10, _model.GameField.Width, 10)); // Низ
            _model.GameObjects.Add(new Wall(0, 0, 10, _model.GameField.Height)); // Лево
            _model.GameObjects.Add(new Wall(_model.GameField.Width - 10, 0, 10, _model.GameField.Height)); // Право

            _view.InvalidateView();
        }
    }
}