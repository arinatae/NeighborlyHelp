using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Tree : GameObject
    {
        public Tree(int x, int y)
        {
            X = x;
            Y = y;
            Width = 30;
            Height = 50;
            Name = "Дерево";
            IsSolid = true; // Сквозь дерево не пройти!
        }

        public override void Draw(Graphics g)
        {
            // Ствол
            Brush trunkBrush = new SolidBrush(Color.FromArgb(101, 67, 33));
            g.FillRectangle(trunkBrush, X + 10, Y + 30, 10, 20);

            // Крона
            Brush leavesBrush = new SolidBrush(Color.FromArgb(34, 139, 34));
            g.FillEllipse(leavesBrush, X, Y, Width, 35);
        }
    }
}
