using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Tree : GameObject
    {
        private const string SpritePath = "Assets/treepicture.png";

        // Жестко задаем размеры как в оригинале
        public Tree(int x, int y) : base(x, y, 195, 200)
        {
            IsSolid = false;
        }

        public override void Draw(Graphics g)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(SpritePath))
                {
                    // Растягиваем картинку до Width и Height объекта
                    g.DrawImage(bmp, X, Y, Width, Height);
                }
            }
            catch
            {
                // Заглушка, если картинки нет
                using (Brush brush = new SolidBrush(Color.ForestGreen))
                {
                    g.FillEllipse(brush, X, Y, Width, Height);
                }
            }
        }
    }
}