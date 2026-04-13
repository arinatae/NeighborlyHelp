namespace NeighborlyHelp.Models
{
    public class GameField
    {
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public string BackgroundColor { get; set; } = "#87CEEB"; // Небесно-голубой

        // Проверка границ поля
        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
