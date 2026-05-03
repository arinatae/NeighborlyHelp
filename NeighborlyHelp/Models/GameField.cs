using System.Windows.Forms;

namespace NeighborlyHelp.Models
{
    public class GameField
    {
        public int Width { get; set; } = Screen.PrimaryScreen.Bounds.Width;
        public int Height { get; set; } = Screen.PrimaryScreen.Bounds.Height;
        public string BackgroundColor { get; set; } = "#87CEEB"; // Небесно-голубой

        // Проверка границ поля
        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
