using BlockyApi.Core;

namespace BlockyApi.Tasks;

public class ResumeBlocking : IBlockyTask
{
    readonly ILogger<ResumeBlocking> _logger;
    readonly IBlockyService _blockyService;

    public ResumeBlocking(ILogger<ResumeBlocking> logger, IBlockyService blockyService)
    {
        _logger = logger;
        _blockyService = blockyService;
    }
    
    public string Name => nameof(ResumeBlocking);
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var resumed = await _blockyService.ResumeBlockingAsync();
        _logger.LogInformation("Resumed blocking, Resumed? : {Resumed}", resumed);
    }
}