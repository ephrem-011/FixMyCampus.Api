using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Stores the hashed password, never the plain-text password.
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Ticket> ReportedTickets { get; set; }
        = new List<Ticket>();

    public ICollection<Ticket> AssignedTickets { get; set; }
        = new List<Ticket>();

    public ICollection<TicketHistory> TicketHistories { get; set; }
        = new List<TicketHistory>();
}