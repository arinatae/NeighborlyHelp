using NeighborlyHelp.Models;
using System.Collections.Generic;

namespace NeighborlyHelp.Managers
{
    public class QuestManager
    {
        public List<Quest> Quests { get; set; } = new List<Quest>();

        public QuestManager()
        {
            // Инициализируем квесты
            Quests.Add(new Quest("keys", "Потерянные ключи", "Найти ключи у скамейки"));
            Quests.Add(new Quest("flowers", "Полить цветы", "Найти лейку и полить цветы"));
        }

        public void CompleteQuest(string questId)
        {
            var quest = Quests.Find(q => q.Id == questId);
            if (quest != null && quest.Status != QuestStatus.Completed)
            {
                quest.Status = QuestStatus.Completed;
            }
        }

        public bool IsQuestCompleted(string questId)
        {
            var quest = Quests.Find(q => q.Id == questId);
            return quest != null && quest.Status == QuestStatus.Completed;
        }
    }
}
