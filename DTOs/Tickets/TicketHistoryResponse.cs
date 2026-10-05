using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.DTOs.Tickets;

public class TicketHistoryResponse
{
    public int Id { get; set; }

    public TicketStatus FromStatus { get; set; }

    public TicketStatus ToStatus { get; set; }

    public int ChangedById { get; set; }

    public string ChangedByName { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }
}