using System.Collections.Generic;
using System.Linq;

namespace NeighborlyHelp.Models
{
    public class Inventory
    {
        public List<Item> Items { get; set; } = new List<Item>();

        public void Add(Item item)
        {
            Items.Add(item);
        }

        public bool HasItem(string itemName)
        {
            return Items.Any(i => i.Name == itemName);
        }

        public string GetList()
        {
            if (Items.Count == 0) return "Пусто";
            return string.Join(", ", Items.Select(i => i.Name));
        }
    }
}
