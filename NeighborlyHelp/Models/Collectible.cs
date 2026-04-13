using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Collectible : GameObject
    {
        public Item Item { get; set; }
        public bool IsPickedUp { get; set; } = false;

        public Collectible(int x, int y, Item item)
        {
            X = x;
            Y = y;
            Width = 15;
            Height = 15;
            Item = item;
        }

        public override void Draw(Graphics g)
        {
            if (IsPickedUp) return;

            // Рисуем предмет как маленький кружок
            Brush brush = new SolidBrush(Item.IconColor);
            g.FillEllipse(brush, X, Y, Width, Height);

            // Обводка
            Pen pen = new Pen(Color.Black, 1);
            g.DrawEllipse(pen, X, Y, Width, Height);
        }
    }
}