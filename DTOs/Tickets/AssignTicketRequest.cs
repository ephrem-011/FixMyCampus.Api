using System.ComponentModel.DataAnnotations;

namespace FixMyCampus.Api.DTOs.Tickets;

public class AssignTicketRequest
{
    [Required]
    public int TechnicianId { get; set; }
}