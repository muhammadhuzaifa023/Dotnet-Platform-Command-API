namespace ApiBook.Domain.Entities;

public class Command
{
    public int Id { get; private set; }

    public string HowTo { get; private set; } = string.Empty;

    public string CommandLine { get; private set; } = string.Empty;

    public int PlatformId { get; private set; }

    public Platform? Platform { get; private set; }

    private Command() { } // EF Core

    public Command(string howTo, string commandLine, int platformId)
    {
        SetHowTo(howTo);
        SetCommandLine(commandLine);

        if (platformId <= 0)
            throw new ArgumentException("Invalid PlatformId");

        PlatformId = platformId;
    }

    public void Update(string howTo, string commandLine, int platformId)
    {
        SetHowTo(howTo);
        SetCommandLine(commandLine);

        if (platformId <= 0)
            throw new ArgumentException("Invalid PlatformId");

        PlatformId = platformId;
    }

    private void SetHowTo(string howTo)
    {
        if (string.IsNullOrWhiteSpace(howTo))
            throw new ArgumentException("HowTo cannot be empty");

        if (howTo.Length > 200)
            throw new ArgumentException("HowTo too long");

        HowTo = howTo;
    }

    private void SetCommandLine(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
            throw new ArgumentException("CommandLine cannot be empty");

        CommandLine = commandLine;
    }
}
