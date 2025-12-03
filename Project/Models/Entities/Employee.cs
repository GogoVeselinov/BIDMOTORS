namespace Project.Models.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
