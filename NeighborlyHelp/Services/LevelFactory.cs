using System;
using System.Collections.Generic;
using NeighborlyHelp.Models;

namespace NeighborlyHelp.Services
{
    public static class LevelFactory
    {
        public static void GenerateLevel(GameModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            model.GameField = new GameField();
            model.Player = new Player(530, 450) { Width = 200, Height = 200 };

            try { model.PlayerSprite = new Bitmap("Assets/sprite0.png"); } catch { }
            try { model.BackgroundImage = new Bitmap("Assets/backpicture.png"); } catch { }
            try { model.BoxSprite = new Bitmap("Assets/sprite-box.png"); } catch { }
            try { model.FlowerSprite = new Bitmap("Assets/spriteflower.png"); } catch { }
            try { model.PlayerPortrait = new Bitmap("Assets/portrait0.png"); } catch { }

            model.GameObjects.Add(new Tree(225, 15));
            model.GameObjects.Add(new Tree(800, 150));
            model.GameObjects.Add(new Tree(500, 800));
            model.GameObjects.Add(new Tree(1200, 730));
            model.GameObjects.Add(new Bench(800, 700));
            model.GameObjects.Add(new Bench(100, 330));
            model.GameObjects.Add(new FlowerBed(40, 450));
            model.GameObjects.Add(new Mailbox(1150, 45));

            model.GameObjects.Add(new Wall(0, 0, model.GameField.Width, 10));
            model.GameObjects.Add(new Wall(0, model.GameField.Height - 10, model.GameField.Width, 10));
            model.GameObjects.Add(new Wall(0, 0, 10, model.GameField.Height));
            model.GameObjects.Add(new Wall(model.GameField.Width - 10, 0, 10, model.GameField.Height));
        }
    }
}