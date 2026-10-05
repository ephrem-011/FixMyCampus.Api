using System.ComponentModel.DataAnnotations;
using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.DTOs.Tickets;

public class UpdateTicketStatusRequest
{
    [Required]
    public TicketStatus Status { get; set; }
}