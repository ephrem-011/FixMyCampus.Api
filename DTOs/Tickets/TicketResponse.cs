using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.DTOs.Tickets;

public class TicketResponse
{
    public int Id { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Room { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; }

    public int BuildingId { get; set; }

    public string BuildingName { get; set; } = string.Empty;

    public int ReporterId { get; set; }

    public string ReporterName { get; set; } = string.Empty;

    public int? TechnicianId { get; set; }

    public string? TechnicianName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<TicketHistoryResponse> History { get; set; }
        = new();
}