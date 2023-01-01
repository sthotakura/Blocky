namespace BlockyApi.Core;

public sealed class BlockyService : IBlockyService, IDisposable
{
    readonly ILogger<BlockyService> _logger;
    readonly IHostsFileReaderWriter _readerWriter;
    readonly IHostsFileLineParser _parser;
    readonly IFlushDns _flushDns;
    readonly List<IHostsFileLine> _lines = new();
    readonly SemaphoreSlim _linesListLock = new(1, 1);
    readonly ISet<string> _pausedSet = new HashSet<string>();
    readonly SemaphoreSlim _pausedSetLock = new(1, 1);

    public BlockyService(ILogger<BlockyService> logger, IHostsFileReaderWriter readerWriter,
        IHostsFileLineParser parser, IFlushDns flushDns)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _readerWriter = readerWriter ?? throw new ArgumentNullException(nameof(readerWriter));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _flushDns = flushDns ?? throw new ArgumentNullException(nameof(flushDns));

        _readerWriter.LinesChanged += async (_, lines) =>
        {
            await _linesListLock.WaitAsync();
            try
            {
                UpdateLines(lines);
            }
            finally
            {
                _linesListLock.Release();
            }
        };
    }

    public async Task<IReadOnlyCollection<string>> GetBlockedListAsync()
    {
        await _linesListLock.WaitAsync();
        try
        {
            UpdateLines(await _readerWriter.ReadLinesAsync());
            return _lines.Where(l => l.IsAddedByBlocky).Select(l => l.Host!).ToList().AsReadOnly();
        }
        finally
        {
            _linesListLock.Release();
        }
    }

    public async Task<bool> BlockAsync(string hostName, string ipAddress = "127.0.0.1")
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));
        if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

        var line = $"{ipAddress} {hostName} {Consts.BlockyComment}";

        await _linesListLock.WaitAsync();
        try
        {
            _logger.LogInformation("Adding {Line}", line);
            UpdateLines(await _readerWriter.ReadLinesAsync());
            _lines.Add(_parser.Parse(line));
            await _readerWriter.WriteLinesAsync(_lines);
            return await _flushDns.FlushAsync();
        }
        finally
        {
            _linesListLock.Release();
        }
    }

    void UpdateLines(IEnumerable<IHostsFileLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
    }

    public async Task<bool> UnblockAsync(string hostName)
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));

        _logger.LogInformation("Removing entry {HostName}", hostName);

        await _linesListLock.WaitAsync();
        try
        {
            UpdateLines(await _readerWriter.ReadLinesAsync());
            _lines.RemoveAll(l => l.Host == hostName && l.IsAddedByBlocky);
            await _readerWriter.WriteLinesAsync(_lines);
            return await _flushDns.FlushAsync();
        }
        finally
        {
            _linesListLock.Release();
        }
    }

    public async Task<bool> PauseBlockingAsync(IEnumerable<string> hostNames)
    {
        var paused = true;
        
        foreach (var hostName in hostNames)
        {
            var hostPaused = await UnblockAsync(hostName);
            if (hostPaused)
            {
                _pausedSet.Add(hostName);
            }
            paused &= hostPaused;
        }

        return paused;
    }

    public async Task<bool> ResumeBlockingAsync()
    {
        var unPaused = true;

        foreach (var pausedHost in _pausedSet)
        {
            var hostUnPaused = await BlockAsync(pausedHost);
            _pausedSet.Remove(pausedHost);
            unPaused &= hostUnPaused;
        }

        return unPaused;
    }

    public void Dispose()
    {
        _linesListLock.Dispose();
        (_readerWriter as IDisposable)?.Dispose();
    }
}