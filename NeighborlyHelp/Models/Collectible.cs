using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Collectible : GameObject
    {
        public Item Item { get; set; }
        public bool IsPickedUp { get; set; } = false;
        public Bitmap? Sprite { get; set; }

        public Collectible(int x, int y, Item item, string spriteFileName = "")
        {
            X = x;
            Y = y;
            Item = item;
            IsSolid = false;

            // Размеры по умолчанию (если картинка не загрузится)
            Width = 30;
            Height = 30;

            if (!string.IsNullOrEmpty(spriteFileName))
            {
                try
                {
                    Sprite = new Bitmap($"Assets/{spriteFileName}");
                    // Автоматически подстраиваем хитбокс под размер картинки
                    // Bounds сам обновится, так как он использует Width и Height
                    Width = 60;
                    Height = 50;
                }
                catch
                {
                    Sprite = null;
                }
            }
        }

        public override void Draw(Graphics g)
        {
            if (IsPickedUp) return;

            if (Sprite != null)
            {
                // Рисуем PNG с прозрачным фоном
                g.DrawImage(Sprite, X, Y, Width, Height);
            }
            else
            {
                // Запасной вариант: старый желтый кружок
                using (Brush brush = new SolidBrush(Color.Gold))
                {
                    g.FillEllipse(brush, X, Y, Width, Height);
                }
            }
        }
    }
}