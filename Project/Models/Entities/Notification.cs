namespace Project.Models.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }

        // Foreign keys
        public int ClientId { get; set; }

        // Navigation properties
        public Client Client { get; set; } = null!;
    }
}
