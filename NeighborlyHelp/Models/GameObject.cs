using System.Drawing;

namespace NeighborlyHelp.Models
{
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 20;
        public int Height { get; set; } = 20;
        public string Name { get; set; } = "Object";
        public bool IsSolid { get; set; } = false; // Можно ли проходить сквозь

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public virtual void Draw(Graphics g)
        {
            // Базовая отрисовка (переопределяется в наследниках)
            Brush brush = new SolidBrush(Color.Gray);
            g.FillRectangle(brush, X, Y, Width, Height);
        }

        public bool CollidesWith(GameObject other)
        {
            return this.Bounds.IntersectsWith(other.Bounds);
        }
    }
}