namespace BlockyWeb.Core;

public sealed class HostFileLine
{
    public HostFileLine(string? line)
    {
        Line = line;
    }

    public string? Line { get; }

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

        return this;
    }
}