namespace BlockyApi.Core;

public sealed class HostsFileLine : IHostsFileLine
{
    public HostsFileLine(string? line)
    {
        Line = line;
    }

    public string? Line { get; }

    public bool IsEmpty { get; private set; }

    public bool IsComment { get; private set; }

    public string? Host { get; private set; }

    public string? IpAddress { get; private set; }

    public bool IsAddedByBlocky => Line?.Contains(Consts.BlockyComment) ?? false;

    public HostsFileLine SetIsEmpty(bool isEmpty)
    {
        IsEmpty = isEmpty;

        return this;
    }

    public HostsFileLine SetIsComment(bool isComment)
    {
        IsComment = isComment;

        return this;
    }

    public HostsFileLine SetHost(string host)
    {
        Host = host;

        return this;
    }

    public HostsFileLine SetIpAddress(string ipAddress)
    {
        IpAddress = ipAddress;

        return this;
    }
}