namespace ApiBook.Domain.Entities;

public class Platform
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public ICollection<Command> Commands { get; set; } = new List<Command>();
}
