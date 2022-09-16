namespace BlockyWeb.Core;

public sealed class HostFileLineParser : IHostFileLineParser
{
    public HostFileLine Parse(string? line)
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

        return parts.Length == 0
            ? new HostFileLine(line).SetIsEmpty(true)
            : new HostFileLine(line).SetHost(parts[0]).SetIpAddress(parts[1]);
    }
}