using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Wall : GameObject
    {
        public Wall(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = "Забор";
            IsSolid = true;
        }

        public override void Draw(Graphics g)
        {
            Brush wallBrush = new SolidBrush(Color.FromArgb(128, 128, 128));
            g.FillRectangle(wallBrush, X, Y, Width, Height);

            // Текстура забора
            Pen linePen = new Pen(Color.FromArgb(100, 100, 100), 1);
            for (int i = 0; i < Height; i += 10)
            {
                g.DrawLine(linePen, X, Y + i, X + Width, Y + i);
            }
        }
    }
}