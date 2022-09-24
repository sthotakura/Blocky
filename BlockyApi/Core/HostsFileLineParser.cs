namespace BlockyApi.Core;

public sealed class HostsFileLineParser : IHostsFileLineParser
{
    public IHostsFileLine Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new HostsFileLine(line).SetIsEmpty(true);
        }

        if (line.TrimStart().StartsWith("#"))
        {
            return new HostsFileLine(line).SetIsComment(true);
        }

        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        return parts.Length == 0
            ? new HostsFileLine(line).SetIsEmpty(true)
            : new HostsFileLine(line).SetIpAddress(parts[0]).SetHost(parts[1]);
    }
}