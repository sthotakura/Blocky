namespace BlockyWeb.Core;

public interface IHostFileLineParser
{
    HostFileLine Parse(string? line);
}