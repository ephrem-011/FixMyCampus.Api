using FixMyCampus.Api.Data;
using FixMyCampus.Api.DTOs.Tickets;
using FixMyCampus.Api.Enums;
using FixMyCampus.Api.Models;
using FixMyCampus.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FixMyCampus.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;

    public TicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TicketResponse> CreateTicketAsync(
        int reporterId,
        CreateTicketRequest request)
    {
        var buildingExists = await _context.Buildings
            .AnyAsync(x => x.Id == request.BuildingId);

        if (!buildingExists)
        {
            throw new KeyNotFoundException("Building not found.");
        }

        var reporter = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Id == reporterId &&
                x.Role == UserRole.Reporter);

        if (reporter is null)
        {
            throw new UnauthorizedAccessException(
                "Only reporters can create tickets.");
        }

        var ticket = new Ticket
        {
            Category = request.Category.Trim(),
            Room = request.Room.Trim(),
            Description = request.Description.Trim(),
            Status = TicketStatus.New,
            BuildingId = request.BuildingId,
            ReporterId = reporterId,
            TechnicianId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();

        return await GetTicketByIdAsync(ticket.Id);
    }

    public async Task<List<TicketResponse>> GetTicketsAsync(
        string? building,
        TicketStatus? status)
    {
        var query = _context.Tickets
            .AsNoTracking()
            .Include(x => x.Building)
            .Include(x => x.Reporter)
            .Include(x => x.Technician)
            .Include(x => x.History)
                .ThenInclude(x => x.ChangedBy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(building))
        {
            query = query.Where(x =>
                x.Building.Name.ToLower()
                    .Contains(building.Trim().ToLower()));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var tickets = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return tickets.Select(MapTicket).ToList();
    }

    public async Task<List<TicketResponse>> GetMyTicketsAsync(
        int reporterId)
    {
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.ReporterId == reporterId)
            .Include(x => x.Building)
            .Include(x => x.Reporter)
            .Include(x => x.Technician)
            .Include(x => x.History)
                .ThenInclude(x => x.ChangedBy)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return tickets.Select(MapTicket).ToList();
    }

    public async Task<TicketResponse> GetTicketByIdAsync(
        int ticketId)
    {
        var ticket = await _context.Tickets
            .AsNoTracking()
            .Include(x => x.Building)
            .Include(x => x.Reporter)
            .Include(x => x.Technician)
            .Include(x => x.History)
                .ThenInclude(x => x.ChangedBy)
            .FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket not found.");
        }

        return MapTicket(ticket);
    }

    public async Task AssignTicketAsync(
        int ticketId,
        int technicianId,
        int adminId)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket not found.");
        }

        var admin = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Id == adminId &&
                x.Role == UserRole.Admin);

        if (admin is null)
        {
            throw new UnauthorizedAccessException(
                "Only admins can assign tickets.");
        }

        if (ticket.Status != TicketStatus.New)
        {
            throw new ArgumentException(
                "Only new tickets can be assigned.");
        }

        var technician = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Id == technicianId &&
                x.Role == UserRole.Technician);

        if (technician is null)
        {
            throw new KeyNotFoundException(
                "Technician not found.");
        }

        var oldStatus = ticket.Status;

        ticket.TechnicianId = technicianId;
        ticket.Status = TicketStatus.Assigned;
        ticket.UpdatedAt = DateTime.UtcNow;

        var history = new TicketHistory
        {
            TicketId = ticket.Id,
            FromStatus = oldStatus,
            ToStatus = TicketStatus.Assigned,
            ChangedById = adminId,
            ChangedAt = DateTime.UtcNow
        };

        _context.TicketHistories.Add(history);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(
        int ticketId,
        TicketStatus newStatus,
        int changedById)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket not found.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == changedById);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "User not found.");
        }

        if (ticket.Status == TicketStatus.Resolved)
        {
            throw new ArgumentException(
                "A resolved ticket cannot be changed.");
        }

        if (user.Role == UserRole.Technician &&
            ticket.TechnicianId != user.Id)
        {
            throw new UnauthorizedAccessException(
                "You are not assigned to this ticket.");
        }

        if (user.Role != UserRole.Admin &&
            user.Role != UserRole.Technician)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to change ticket status.");
        }

        var validTransition =
            ticket.Status switch
            {
                TicketStatus.Assigned =>
                    newStatus == TicketStatus.InProgress,

                TicketStatus.InProgress =>
                    newStatus == TicketStatus.Resolved,

                _ => false
            };

        if (!validTransition)
        {
            throw new ArgumentException(
                $"Invalid status transition: " +
                $"{ticket.Status} → {newStatus}.");
        }

        var oldStatus = ticket.Status;

        ticket.Status = newStatus;
        ticket.UpdatedAt = DateTime.UtcNow;

        var history = new TicketHistory
        {
            TicketId = ticket.Id,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            ChangedById = changedById,
            ChangedAt = DateTime.UtcNow
        };

        _context.TicketHistories.Add(history);

        await _context.SaveChangesAsync();
    }

    public async Task<List<BuildingResponse>> GetBuildingsAsync()
    {
        return await _context.Buildings
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new BuildingResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<List<UserResponse>> GetTechniciansAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Role == UserRole.Technician)
            .OrderBy(x => x.Name)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role
            })
            .ToListAsync();
    }

    private static TicketResponse MapTicket(Ticket ticket)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            Category = ticket.Category,
            Room = ticket.Room,
            Description = ticket.Description,
            Status = ticket.Status,
            BuildingId = ticket.BuildingId,
            BuildingName = ticket.Building.Name,
            ReporterId = ticket.ReporterId,
            ReporterName = ticket.Reporter.Name,
            TechnicianId = ticket.TechnicianId,
            TechnicianName = ticket.Technician?.Name,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,

            History = ticket.History
                .OrderBy(x => x.ChangedAt)
                .Select(x => new TicketHistoryResponse
                {
                    Id = x.Id,
                    FromStatus = x.FromStatus,
                    ToStatus = x.ToStatus,
                    ChangedById = x.ChangedById,
                    ChangedByName = x.ChangedBy.Name,
                    ChangedAt = x.ChangedAt
                })
                .ToList()
        };
    }
}