using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Color IconColor { get; set; } // Для простой отрисовки

        public Item(string name, string desc, Color color)
        {
            Name = name;
            Description = desc;
            IconColor = color;
        }
    }
}
