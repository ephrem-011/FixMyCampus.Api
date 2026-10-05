using System.Security.Claims;
using FixMyCampus.Api.DTOs.Tickets;
using FixMyCampus.Api.Enums;
using FixMyCampus.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixMyCampus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Reporter,Technician")]
    public async Task<ActionResult<List<TicketResponse>>> GetTickets(
        [FromQuery] string? building,
        [FromQuery] TicketStatus? status)
    {
        var tickets = await _ticketService.GetTicketsAsync(
            building,
            status);

        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Reporter,Technician")]
    public async Task<ActionResult<TicketResponse>> GetTicket(
        int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);

        return Ok(ticket);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Reporter")]
    public async Task<ActionResult<List<TicketResponse>>> GetMyTickets()
    {
        var userId = GetCurrentUserId();

        var tickets = await _ticketService.GetMyTicketsAsync(userId);

        return Ok(tickets);
    }

    [HttpPost]
    [Authorize(Roles = "Reporter")]
    public async Task<ActionResult<TicketResponse>> CreateTicket(
        CreateTicketRequest request)
    {
        var reporterId = GetCurrentUserId();

        var ticket = await _ticketService.CreateTicketAsync(
            reporterId,
            request);

        return CreatedAtAction(
            nameof(GetTicket),
            new { id = ticket.Id },
            ticket);
    }

    [HttpPut("{id:int}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTicket(
        int id,
        AssignTicketRequest request)
    {
        var adminId = GetCurrentUserId();

        await _ticketService.AssignTicketAsync(
            id,
            request.TechnicianId,
            adminId);

        return Ok(new
        {
            message = "Ticket assigned successfully."
        });
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateTicketStatusRequest request)
    {
        var userId = GetCurrentUserId();

        await _ticketService.UpdateStatusAsync(
            id,
            request.Status,
            userId);

        return Ok(new
        {
            message = "Ticket status updated successfully."
        });
    }

    [HttpGet("buildings")]
    [Authorize(Roles = "Admin,Reporter,Technician")]
    public async Task<ActionResult<List<BuildingResponse>>> GetBuildings()
    {
        var buildings = await _ticketService.GetBuildingsAsync();

        return Ok(buildings);
    }

    [HttpGet("technicians")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserResponse>>> GetTechnicians()
    {
        var technicians = await _ticketService.GetTechniciansAsync();

        return Ok(technicians);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid authentication token.");
        }

        return userId;
    }
}