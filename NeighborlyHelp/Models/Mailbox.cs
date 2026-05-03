using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Mailbox : GameObject
    {
        private Bitmap? sprite;

        public Mailbox(int x, int y)
        {
            X = x;
            Y = y;

            // Размеры зоны клика/коллизии (подбери под свою картинку)
            Width = 120;
            Height = 140;

            Name = "Mailbox";
            IsSolid = false; // Ящик обычно непроходимый

            // Загрузка PNG
            try
            {
                sprite = new Bitmap("Assets/postpicture.png");
            }
            catch
            {
                sprite = null;
            }
        }

        public override void Draw(Graphics g)
        {
            if (sprite != null)
            {
                // Рисуем картинку с прозрачностью
                g.DrawImage(sprite, X, Y, Width, Height);
            }
            else
            {
                // Запасной вариант, если картинка не загрузилась
                g.FillRectangle(Brushes.DarkBlue, X, Y, Width, Height);
                g.DrawRectangle(Pens.White, X, Y, Width, Height);
            }
        }
    }
}