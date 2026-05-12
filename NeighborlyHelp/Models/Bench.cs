using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Bench : GameObject
    {
        private const string SpritePath = "Assets/skameyka.png";

        public Bench(int x, int y) : base(x, y, 180, 90)
        {
            IsSolid = false;
        }

        public override void Draw(Graphics g)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(SpritePath))
                {
                    g.DrawImage(bmp, X, Y, Width, Height);
                }
            }
            catch
            {
                using (Brush brush = new SolidBrush(Color.SaddleBrown))
                {
                    g.FillRectangle(brush, X, Y, Width, Height);
                }
            }
        }
    }
}