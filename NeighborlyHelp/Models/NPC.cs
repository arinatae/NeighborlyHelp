using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class NPC : GameObject
    {
        public string DisplayName { get; set; }
        public List<string> DialogLines { get; set; }
        public bool IsDialogAvailable { get; set; } = true;

        public NPC(int x, int y, string name, List<string> dialog)
        {
            X = x;
            Y = y;
            Width = 20;
            Height = 20;
            Name = "NPC";
            DisplayName = name;
            DialogLines = dialog;
            IsSolid = false;
        }

        public override void Draw(Graphics g)
        {
            // Тело персонажа
            Brush bodyBrush = new SolidBrush(Color.FromArgb(255, 182, 193)); // Розовый
            g.FillEllipse(bodyBrush, X, Y, Width, Height);

            // Лицо
            Brush faceBrush = new SolidBrush(Color.FromArgb(255, 228, 196));
            g.FillEllipse(faceBrush, X + 4, Y + 2, 12, 12);

            // Индикатор диалога (!)
            if (IsDialogAvailable)
            {
                Brush exclaimBrush = new SolidBrush(Color.Yellow);
                Pen exclaimPen = new Pen(Color.Black, 1);
                g.FillEllipse(exclaimBrush, X + 15, Y - 10, 12, 12);
                g.DrawEllipse(exclaimPen, X + 15, Y - 10, 12, 12);
                g.DrawString("!", new Font("Arial", 8, FontStyle.Bold),
                    Brushes.Black, X + 18, Y - 9);
            }
        }
    }
}