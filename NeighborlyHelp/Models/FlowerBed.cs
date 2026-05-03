using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class FlowerBed : GameObject
    {
        private Bitmap? sprite;

        public FlowerBed(int x, int y)
        {
            X = x;
            Y = y;

            // Установи размеры зоны клика/границ под твою картинку
            Width = 220;  // Ширина клумбы
            Height = 180; // Высота клумбы

            Name = "FlowerBed";
            IsSolid = false; // Клумба обычно проходимая

            // Загрузка PNG-картинки
            try
            {
                sprite = new Bitmap("Assets/klumbapicture.png");
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
                // Рисуем картинку клумбы с прозрачностью
                g.DrawImage(sprite, X, Y, Width, Height);
            }
            else
            {
                // Запасной вариант, если картинка не загрузилась
                using (Brush brush = new SolidBrush(Color.FromArgb(100, 160, 100)))
                {
                    g.FillEllipse(brush, X, Y, Width, Height);
                }
            }
        }
    }
}