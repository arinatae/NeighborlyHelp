using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Bench : GameObject
    {
        public Bench(int x, int y)
        {
            X = x;
            Y = y;
            Width = 40;
            Height = 20;
            Name = "Скамейка";
            IsSolid = false; // На скамейку можно сесть, но она не стена
        }

        public override void Draw(Graphics g)
        {
            // Коричневая скамейка
            Brush benchBrush = new SolidBrush(Color.FromArgb(139, 69, 19));
            g.FillRectangle(benchBrush, X, Y, Width, Height);

            // Ножки скамейки
            Pen legPen = new Pen(Color.FromArgb(101, 67, 33), 2);
            g.DrawLine(legPen, X + 5, Y + Height, X + 5, Y + Height + 10);
            g.DrawLine(legPen, X + Width - 5, Y + Height, X + Width - 5, Y + Height + 10);
        }
    }
}
