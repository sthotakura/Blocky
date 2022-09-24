namespace BlockyApi.Core;

public interface IHostsFileLineParser
{
    IHostsFileLine Parse(string? line);
}