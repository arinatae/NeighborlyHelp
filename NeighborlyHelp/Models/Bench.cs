using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Bench : GameObject
    {
        private Bitmap? sprite;

        public Bench(int x, int y)
        {
            X = x;
            Y = y;

            // Установи размеры зоны клика/столкновения под твою картинку.
            // Если картинка больше, измени Width и Height здесь.
            Width = 150;
            Height = 100;

            Name = "Bench";
            IsSolid = false; // Скамейку нельзя проходить насквозь

            // Загрузка PNG-картинки
            try
            {
                sprite = new Bitmap("Assets/skameyka.png");
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
                // Рисуем картинку скамейки
                g.DrawImage(sprite, X, Y, Width, Height);
            }
            else
            {
                // Запасной вариант (если картинка не загрузилась)
                // Рисуем коричневый прямоугольник
                using (Brush b = new SolidBrush(Color.SaddleBrown))
                {
                    g.FillRectangle(b, X, Y, Width, Height);
                }
            }
        }
    }
}