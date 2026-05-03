using System.Collections.Generic;
using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class NPC : GameObject
    {
        public string DisplayName { get; set; }
        public List<string> DialogLines { get; set; }
        public bool IsDialogAvailable { get; set; } = true;
        public Bitmap? SpriteImage { get; set; }
        public string PortraitFileName { get; set; }

        public NPC(int x, int y, string name, List<string> dialog, string imagePath, int width = 100, int height = 100, string portraitFile = "")
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = "NPC";
            DisplayName = name;
            DialogLines = dialog;
            IsSolid = false;
            PortraitFileName = portraitFile;

            try { SpriteImage = new Bitmap("Assets/" + imagePath); }
            catch { SpriteImage = null; }
        }

        public override void Draw(Graphics g)
        {
            if (SpriteImage != null)
                g.DrawImage(SpriteImage, X, Y, Width, Height);
            else
            {
                Brush bodyBrush = new SolidBrush(Color.FromArgb(255, 182, 193));
                g.FillEllipse(bodyBrush, X, Y, Width, Height);
            }
        }
    }
}