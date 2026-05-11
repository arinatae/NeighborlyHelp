using System.Collections.Generic;
using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class NPC : GameObject
    {
        public string DisplayName { get; set; }
        public List<string> DialogLines { get; set; }
        public string SpriteName { get; set; }
        public string PortraitFileName { get; set; }
        public bool IsDialogAvailable { get; set; } = true;

        public NPC(int x, int y, string name, List<string> lines, string sprite, int w, int h, string portrait = "")
            : base(x, y, w, h)
        {
            DisplayName = name;
            DialogLines = lines;
            SpriteName = sprite;
            PortraitFileName = portrait;
        }

        public override void Draw(Graphics g)
        {
            // Загрузка спрайта по имени
            try
            {
                using var bmp = new Bitmap($"Assets/{SpriteName}");
                g.DrawImage(bmp, X, Y, Width, Height);
            }
            catch
            {
                g.FillRectangle(Brushes.Blue, X, Y, Width, Height);
            }
        }
    }
}