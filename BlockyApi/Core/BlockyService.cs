namespace BlockyApi.Core;

public sealed class BlockyService : IBlockyService, IDisposable
{
    readonly ILogger<BlockyService> _logger;
    readonly IHostsFileReaderWriter _readerWriter;
    readonly IHostsFileLineParser _parser;
    readonly IFlushDns _flushDns;
    readonly Task<IReadOnlyCollection<IHostsFileLine>> _getTask;
    readonly List<IHostsFileLine> _lines = new();
    readonly SemaphoreSlim _listLock = new(1, 1);

    public BlockyService(ILogger<BlockyService> logger, IHostsFileReaderWriter readerWriter,
        IHostsFileLineParser parser, IFlushDns flushDns)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _readerWriter = readerWriter ?? throw new ArgumentNullException(nameof(readerWriter));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _flushDns = flushDns ?? throw new ArgumentNullException(nameof(flushDns));

        _getTask = _readerWriter.ReadLinesAsync();
        _readerWriter.LinesChanged += async (_, lines) =>
        {
            await _listLock.WaitAsync();
            try
            {
                UpdateLines(lines);
            }
            finally
            {
                _listLock.Release();
            }
        };
    }

    public async Task<IReadOnlyCollection<string>> GetBlockedListAsync()
    {
        await _getTask;
        await _listLock.WaitAsync();
        try
        {
            return _lines.Where(l => l.IsAddedByBlocky).Select(l => l.Host!).ToList().AsReadOnly();
        }
        finally
        {
            _listLock.Release();
        }
    }

    public async Task<bool> BlockAsync(string hostName, string ipAddress = "127.0.0.1")
    {
        if (hostName == null) throw new ArgumentNullException(nameof(hostName));
        if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

        var line = $"{ipAddress} {hostName} {Consts.BlockyComment}";

        await _listLock.WaitAsync();
        try
        {
            _logger.LogInformation("Adding {Line}", line);
            UpdateLines(await _getTask);
            _lines.Add(_parser.Parse(line));
            await _readerWriter.WriteLinesAsync(_lines);
            return await _flushDns.FlushAsync();
        }
        finally
        {
            _listLock.Release();
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

        await _listLock.WaitAsync();
        try
        {
            UpdateLines(await _getTask);
            _lines.RemoveAll(l => l.Host == hostName);
            await _readerWriter.WriteLinesAsync(_lines);
            return await _flushDns.FlushAsync();
        }
        finally
        {
            _listLock.Release();
        }
    }

    public void Dispose()
    {
        _getTask.Dispose();
        _listLock.Dispose();
        (_readerWriter as IDisposable)?.Dispose();
    }
}