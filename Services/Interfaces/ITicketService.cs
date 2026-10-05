using FixMyCampus.Api.DTOs.Tickets;
using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.Services.Interfaces;

public interface ITicketService
{
    Task<TicketResponse> CreateTicketAsync(
        int reporterId,
        CreateTicketRequest request);

    Task<List<TicketResponse>> GetTicketsAsync(
        string? building,
        TicketStatus? status);

    Task<List<TicketResponse>> GetMyTicketsAsync(
        int reporterId);

    Task<TicketResponse> GetTicketByIdAsync(
        int ticketId);

    Task AssignTicketAsync(
        int ticketId,
        int technicianId,
        int adminId);

    Task UpdateStatusAsync(
        int ticketId,
        TicketStatus newStatus,
        int changedById);

    Task<List<BuildingResponse>> GetBuildingsAsync();

    Task<List<UserResponse>> GetTechniciansAsync();
}