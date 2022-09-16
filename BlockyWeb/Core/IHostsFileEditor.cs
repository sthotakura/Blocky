namespace BlockyWeb.Core;

public interface IHostsFileEditor
{
    Task<IHostsFileEditor> AddEntryAsync(string hostName, string ipAddress = "127.0.0.1");
}

public sealed class HostFileLine
{
    public HostFileLine(string line)
    {
        Line = line;
    }

    public string Line { get; }

    public bool IsEmpty { get; private set; }

    public bool IsComment { get; private set; }

    public string? Host { get; private set; }

    public string? IpAddress { get; private set; }

    public HostFileLine SetIsEmpty(bool isEmpty)
    {
        IsEmpty = isEmpty;

        return this;
    }

    public HostFileLine SetIsComment(bool isComment)
    {
        IsComment = isComment;

        return this;
    }

    public HostFileLine SetHost(string host)
    {
        Host = host;

        return this;
    }

    public HostFileLine SetIpAddress(string ipAddress)
    {
        IpAddress = ipAddress;
    }
}

public interface IHostFileLineParser
{
    HostFileLine Parse(string line);
}

public sealed class HostFileLineParser : IHostFileLineParser
{
    public HostFileLine Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new HostFileLine(line).SetIsEmpty(true);
        }

        if (line.TrimStart().StartsWith("#"))
        {
            return new HostFileLine(line).SetIsComment(true);
        }

        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        return parts.Length == 0 ? new HostFileLine(line).SetIsEmpty(true) : new HostFileLine(line).SetHost(parts[0]).SetIpAddress(parts[1]);
    }
}

public sealed class HostFileEditor : IHostsFileEditor
{
    const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
    
    readonly IHostFileLineParser _parser;

    public HostFileEditor(IHostFileLineParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    private async Task LoadAsync()
    {
        using var reader = new StreamReader(File.Open(HostsFilePath, FileMode.Open, FileAccess.Read, ))
    }
    
    public async Task<IHostsFileEditor> AddEntryAsync(string hostName, string ipAddress = "127.0.0.1")
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));
        if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

        return this;
    }
}