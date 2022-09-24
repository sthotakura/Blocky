namespace BlockyApi.Core;

public interface IHostsFileLine
{
    public string? Line { get; }

    public bool IsEmpty { get; }

    public bool IsComment { get; }

    public string? Host { get; }

    public string? IpAddress { get; }

    public bool IsAddedByBlocky { get; }
}