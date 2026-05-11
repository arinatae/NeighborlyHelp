using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Collectible : GameObject
    {
        public Item Item { get; set; }
        public bool IsPickedUp { get; set; } = false;
        private string _spriteName;

        public Collectible(int x, int y, Item item, string spriteName)
            : base(x, y, 50, 50)
        {
            Item = item;
            _spriteName = spriteName;
            IsSolid = false;
        }

        public override void Draw(Graphics g)
        {
            if (!IsPickedUp)
            {
                try
                {
                    using var bmp = new Bitmap($"Assets/{_spriteName}");
                    g.DrawImage(bmp, X, Y, Width, Height);
                }
                catch
                {
                    g.FillEllipse(Brushes.Gold, X, Y, Width, Height);
                }
            }
        }
    }
}