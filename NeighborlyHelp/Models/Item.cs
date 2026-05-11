using System.Drawing;

namespace NeighborlyHelp.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }

        public Item(string name, string desc, Color color)
        {
            Name = name;
            Description = desc;
            Color = color;
        }
    }
}