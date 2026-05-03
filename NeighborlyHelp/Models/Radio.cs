using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Radio : GameObject
    {
        private Bitmap? sprite;

        public Radio(int x, int y)
        {
            X = x;
            Y = y;

            // Установи размеры зоны клика/границ под пропорции твоей картинки
            Width = 100;  // Ширина
            Height = 100; // Высота

            Name = "Radio";
            IsSolid = false; // Поставь true, если радио должно быть препятствием

            // Загрузка PNG-картинки
            try
            {
                sprite = new Bitmap("Assets/radiopicture.png");
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
                // Рисуем картинку радио с прозрачным фоном
                g.DrawImage(sprite, X, Y, Width, Height);
            }
            else
            {
                // Запасной вариант, если картинка не загрузилась
                using (Brush brush = new SolidBrush(Color.DarkGray))
                {
                    g.FillRectangle(brush, X, Y, Width, Height);
                    g.DrawRectangle(Pens.Black, X, Y, Width, Height);
                }
            }
        }
    }
}
