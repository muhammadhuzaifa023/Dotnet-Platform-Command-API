namespace ApiBook.Domain.Entities;

public class Platform
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Publisher { get; private set; } = string.Empty;

    // Expose as read-only to outside
    private readonly List<Command> _commands = new();
    public IReadOnlyCollection<Command> Commands => _commands.AsReadOnly();

    // Audit fields (optional but recommended)
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Soft delete (optional)
    public bool IsDeleted { get; private set; }

    private Platform() { } // EF Core

    public Platform(string name, string publisher)
    {
        SetName(name);
        SetPublisher(publisher);
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string publisher)
    {
        SetName(name);
        SetPublisher(publisher);
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Controlled relationship management
    public void AddCommand(Command command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _commands.Add(command);
    }

    public void RemoveCommand(Command command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        _commands.Remove(command);
    }

    // Private validation methods
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (name.Length > 100)
            throw new ArgumentException("Name too long");

        Name = name;
    }

    private void SetPublisher(string publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            throw new ArgumentException("Publisher is required");

        if (publisher.Length > 100)
            throw new ArgumentException("Publisher too long");

        Publisher = publisher;
    }
}