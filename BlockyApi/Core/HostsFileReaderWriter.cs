namespace BlockyApi.Core;

public sealed class HostsFileReaderWriter : IHostsFileReaderWriter, IDisposable
{
    readonly ILogger<HostsFileReaderWriter> _logger;
    readonly IHostsFileLineParser _parser;
    readonly FileSystemWatcher _watcher;
    List<IHostsFileLine> _lines = new();
    readonly SemaphoreSlim _listLock = new(1, 1);

    Task _loadTask;

    public HostsFileReaderWriter(ILogger<HostsFileReaderWriter> logger, IHostsFileEditorSettings settings, IHostsFileLineParser parser)
    {
        _logger = logger;
        _parser = parser;
        
        HostsFilePath = (settings ?? throw new ArgumentNullException(nameof(settings))).HostsFilePath;

        _watcher = new FileSystemWatcher(Path.GetDirectoryName(HostsFilePath)!, Path.GetFileName(HostsFilePath));
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Changed += async (_, args) =>
        {
            _logger.LogInformation("Changed event received from the file watcher: {ArgsFullPath}", args.FullPath);
            _loadTask = LoadAsync();
            await _loadTask;
            LinesChanged?.Invoke(this, _lines.AsReadOnly());
        };
        _loadTask = LoadAsync();
    }
    
    string HostsFilePath { get; }
    
    async Task LoadAsync()
    {
        _logger.LogInformation("Loading Hosts file from {HostsFilePath}", HostsFilePath);

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

            _logger.LogInformation("Loaded {LinesCount} lines form the Hosts file", _lines.Count);
        }
        finally
        {
            _listLock.Release();
        }
    }

    public async Task<IReadOnlyCollection<IHostsFileLine>> ReadLinesAsync()
    {
        await _loadTask;
        await _listLock.WaitAsync();
        try
        {
            return _lines.AsReadOnly();
        }
        finally
        {
            _listLock.Release();
        }
    }

    public event EventHandler<IReadOnlyCollection<IHostsFileLine>>? LinesChanged;
    
    public async Task WriteLinesAsync(IEnumerable<IHostsFileLine> lines)
    {
        if (lines == null) throw new ArgumentNullException(nameof(lines));

        var toWrite = lines.ToList();
        
        _logger.LogInformation("Saving {LinesCount} to Hosts file: {HostsFilePath}", toWrite.Count, HostsFilePath);
        await File.WriteAllLinesAsync(HostsFilePath, toWrite.Select(l => l.Line ?? string.Empty));
        Interlocked.Exchange(ref _lines, toWrite);
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _listLock.Dispose();
        _loadTask.Dispose();
    }
}