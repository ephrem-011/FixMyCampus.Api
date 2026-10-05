using System.ComponentModel.DataAnnotations;

namespace FixMyCampus.Api.DTOs.Tickets;

public class CreateTicketRequest
{
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Room { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int BuildingId { get; set; }
}