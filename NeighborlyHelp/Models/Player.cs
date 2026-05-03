namespace NeighborlyHelp.Models
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Speed { get; set; } = 5;
        public string Name { get; set; } = "Игрок";

        public int Width { get; set; } = 120;
        public int Height { get; set; } = 120;

        public Player(int startX, int startY)
        {
            X = startX;
            Y = startY;
        }

        public void MoveUp() => Y -= Speed;
        public void MoveDown() => Y += Speed;
        public void MoveLeft() => X -= Speed;
        public void MoveRight() => X += Speed;
    }
}