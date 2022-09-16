using System.Text;

namespace BlockyWeb.Core;

public sealed class HostFileEditor : IHostsFileEditor, IDisposable
{
    const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";

    readonly ILogger<HostFileEditor> _logger;
    readonly IHostFileLineParser _parser;
    readonly FileSystemWatcher _watcher;

    readonly List<HostFileLine> _lines = new();
    readonly SemaphoreSlim _listLock = new(1, 1);
    Task _loadTask;

    public HostFileEditor(ILogger<HostFileEditor> logger, IHostFileLineParser parser)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));

        _watcher = new FileSystemWatcher(Path.GetDirectoryName(HostsFilePath) ?? @"C:\Windows\System32\drivers\etc",
            Path.GetFileName(HostsFilePath));
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Changed += (_, args) =>
        {
            _logger.LogInformation($"Changed event received from the file watcher: {args.FullPath}");
            _loadTask = LoadAsync();
        };

        _loadTask = LoadAsync();
    }

    async Task LoadAsync()
    {
        _logger.LogInformation($"Loading Hosts file from {HostsFilePath}");

        await _listLock.WaitAsync();
        try
        {
            _lines.Clear();

            using var reader =
                new StreamReader(File.Open(HostsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                _lines.Add(_parser.Parse(line));
            }

            _logger.LogInformation($"Loaded {_lines.Count} lines form the Hosts file.");
        }
        finally
        {
            _listLock.Release();
        }
    }

    async Task SaveAsync()
    {
        _logger.LogInformation($"Saving {_lines.Count} to Hosts file: {HostsFilePath}");
        var sb = new StringBuilder();
        await File.WriteAllLinesAsync(HostsFilePath, _lines.Select(l => l.Line ?? string.Empty));
    }

    public async Task<IHostsFileEditor> AddEntryAsync(string hostName, string ipAddress = "127.0.0.1")
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));
        if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

        await _loadTask;
        var line = $"{ipAddress} {hostName} # Added by Blocky";

        await _listLock.WaitAsync();
        try
        {
            _lines.Add(_parser.Parse(line));
            await SaveAsync();
        }
        finally
        {
            _listLock.Release();
        }

        return this;
    }

    public async Task<IHostsFileEditor> RemoveEntryAsync(string hostName)
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));

        await _loadTask;
        await _listLock.WaitAsync();

        try
        {
            _lines.RemoveAll(l => l.Host == hostName);
            await SaveAsync();
        }
        finally
        {
            _listLock.Release();
        }

        return this;
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _loadTask.Dispose();
    }
}