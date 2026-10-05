namespace FixMyCampus.Api.Models;

public class Building
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Ticket> Tickets { get; set; }
        = new List<Ticket>();
}