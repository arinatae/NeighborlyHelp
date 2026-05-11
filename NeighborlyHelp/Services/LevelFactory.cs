using System;
using System.Collections.Generic;
using System.Drawing;
using NeighborlyHelp.Models;

namespace NeighborlyHelp.Services
{
    public static class LevelFactory
    {
        /// <summary>
        /// Генерирует начальное состояние игры: поле, игрока, статические объекты и загружает спрайты.
        /// </summary>
        /// <param name="model">Экземпляр GameModel, который нужно заполнить.</param>
        public static void GenerateLevel(GameModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // 1. Инициализация поля и игрока
            model.GameField = new GameField();
            model.Player = new Player(530, 450)
            {
                Width = 200,
                Height = 200
            };

            // 2. Загрузка основных спрайтов (с обработкой ошибок, если файлы отсутствуют)
            try { model.PlayerSprite = new Bitmap("Assets/sprite0.png"); } catch { }
            try { model.BackgroundImage = new Bitmap("Assets/backpicture.png"); } catch { }
            try { model.BoxSprite = new Bitmap("Assets/sprite-box.png"); } catch { }
            try { model.FlowerSprite = new Bitmap("Assets/spriteflower.png"); } catch { }
            try { model.PlayerPortrait = new Bitmap("Assets/portrait0.png"); } catch { }

            // 3. Очистка списков объектов (на случай повторного вызова)
            model.GameObjects.Clear();
            model.NPCs.Clear();
            model.Collectibles.Clear();

            // 4. Создание статических объектов окружения
            // Деревья
            model.GameObjects.Add(new Tree(225, 15));
            model.GameObjects.Add(new Tree(800, 150));
            model.GameObjects.Add(new Tree(500, 800));
            model.GameObjects.Add(new Tree(1200, 730));

            // Скамейки
            model.GameObjects.Add(new Bench(800, 700));
            model.GameObjects.Add(new Bench(100, 330));

            // Клумба (статический объект, мини-игра создается отдельно)
            model.GameObjects.Add(new FlowerBed(40, 450));

            // Почтовый ящик
            model.GameObjects.Add(new Mailbox(1150, 45));

            // 5. Создание границ уровня (Стены)
            // Верхняя стена
            model.GameObjects.Add(new Wall(0, 0, model.GameField.Width, 10));
            // Нижняя стена
            model.GameObjects.Add(new Wall(0, model.GameField.Height - 10, model.GameField.Width, 10));
            // Левая стена
            model.GameObjects.Add(new Wall(0, 0, 10, model.GameField.Height));
            // Правая стена
            model.GameObjects.Add(new Wall(model.GameField.Width - 10, 0, 10, model.GameField.Height));

            // Примечание: NPC (Шарлотта, Оливер и т.д.) создаются не здесь, а в QuestService.StartStory(),
            // так как они являются частью динамического сюжета.
        }
    }
}