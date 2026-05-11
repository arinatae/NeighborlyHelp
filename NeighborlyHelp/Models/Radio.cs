using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Radio : GameObject
    {
        private const string SpritePath = "Assets/radiopicture.png";

        public Radio(int x, int y) : base(x, y, 80, 60)
        {
            IsSolid = true;
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
                using (Brush body = new SolidBrush(Color.DarkSlateGray))
                {
                    g.FillRectangle(body, X, Y, Width, Height);
                }
                using (Pen pen = new Pen(Color.Silver, 2))
                {
                    g.DrawRectangle(pen, X, Y, Width, Height);
                }
            }
        }
    }
}