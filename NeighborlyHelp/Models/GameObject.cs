using System.Drawing;

namespace NeighborlyHelp.Models
{
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsSolid { get; set; } = true;

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        protected GameObject(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public abstract void Draw(Graphics g);
    }
}