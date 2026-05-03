using System.Collections.Generic;
using System.Text;
using NeighborlyHelp.Models;

namespace NeighborlyHelp
{
    public class Inventory
    {
        private List<Item> items = new List<Item>();

        public void Add(Item item)
        {
            items.Add(item);
        }

        public string GetList()
        {
            if (items.Count == 0)
                return "Пусто";

            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine($"• {item.Name}");
            }
            return sb.ToString();
        }

        // Проверка наличия предмета по имени
        public bool Contains(string itemName)
        {
            return items.Exists(i => i.Name == itemName);
        }

        // Удаление предмета по имени
        public bool Remove(string itemName)
        {
            var item = items.Find(i => i.Name == itemName);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }
}
