using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class FlowerBed : GameObject
    {
        // Убедись, что имя файла совпадает с твоим в Assets!
        private const string SpritePath = "Assets/klumbapicture.png";

        public FlowerBed(int x, int y) : base(x, y, 230, 180)
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
                using (Brush brush = new SolidBrush(Color.BurlyWood))
                {
                    g.FillEllipse(brush, X, Y, Width, Height);
                }
            }
        }
    }
}