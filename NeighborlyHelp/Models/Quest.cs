namespace NeighborlyHelp.Models
{
    public enum QuestStatus { New, InProgress, Completed }

    public class Quest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; set; }
        public bool IsHidden { get; set; } = false;

        public Quest(string id, string title, string desc)
        {
            Id = id;
            Title = title;
            Description = desc;
            Status = QuestStatus.New;
        }
    }
}
