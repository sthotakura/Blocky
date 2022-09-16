namespace BlockyWeb.Core;

public interface IHostsFileEditor
{
    Task<IHostsFileEditor> AddEntryAsync(string hostName, string ipAddress = "127.0.0.1");

    Task<IHostsFileEditor> RemoveEntryAsync(string hostName);
}