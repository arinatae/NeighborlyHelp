using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Mailbox : GameObject
    {
        private const string SpritePath = "Assets/postpicture.png";

        public Mailbox(int x, int y) : base(x, y, 140, 160)
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
                using (Brush box = new SolidBrush(Color.Blue))
                {
                    g.FillRectangle(box, X, Y, Width, 50);
                }
                using (Brush pole = new SolidBrush(Color.Gray))
                {
                    g.FillRectangle(pole, X + 20, Y + 50, 20, 50);
                }
            }
        }
    }
}