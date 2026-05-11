using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Wall : GameObject
    {
        public Wall(int x, int y, int width, int height) : base(x, y, width, height)
        {
            IsSolid = true; // Стены всегда твердые
        }

        public override void Draw(Graphics g)
        {
            // Стены обычно невидимы или имеют текстуру травы/забора по краям.
            // Для отладки можно раскомментировать строку ниже, чтобы видеть границы:

            // using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.Green)))
            // {
            //     g.FillRectangle(brush, X, Y, Width, Height);
            // }

            // В финальной версии стены часто не рисуются вообще, если они совпадают с краем экрана,
            // или рисуются как декоративный забор.
            // Здесь оставим пустым или нарисуем тонкую линию, если это внутренний объект.
        }
    }
}