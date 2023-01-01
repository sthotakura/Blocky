using BlockyApi.Core;

namespace BlockyApi.Tasks;

public class PauseBlocking : IBlockyTask
{
    readonly ILogger<PauseBlocking> _logger;
    readonly IBlockyService _blockyService;

    public PauseBlocking(ILogger<PauseBlocking> logger, IBlockyService blockyService)
    {
        _logger = logger;
        _blockyService = blockyService;
    }

    public string Name => nameof(PauseBlocking);

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var blocked = await _blockyService.GetBlockedListAsync();
        var paused = await _blockyService.PauseBlockingAsync(blocked);
        _logger.LogInformation("Paused blocking for {HostNames}, Paused? : {Paused}", string.Join(",", blocked), paused);
    }
}