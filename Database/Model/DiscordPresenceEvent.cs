using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wa77cher.Database.Model
{
    public enum PresenceEventType
    {
        Start,
        End
    }

    internal class DiscordPresenceEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ActivityName { get; set; }
        public PresenceEventType EventType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
