using BlockyApi.Core;

namespace BlockyApi.Tasks;

public class BlockyTaskScheduler : IBlockyTaskScheduler
{
    readonly ILogger<BlockyTaskScheduler> _logger;
    readonly IDateTimeService _dateTimeService;

    public BlockyTaskScheduler(ILogger<BlockyTaskScheduler> logger, IDateTimeService dateTimeService)
    {
        _logger = logger;
        _dateTimeService = dateTimeService;
    }

    public void Schedule(IBlockyTask blockyTask, IBlockyTaskSchedule schedule, CancellationToken cancellationToken)
    {
        if (blockyTask == null) throw new ArgumentNullException(nameof(blockyTask));
        if (schedule == null) throw new ArgumentNullException(nameof(schedule));

        var now = _dateTimeService.Now;
        var today = _dateTimeService.Today;
        var runAt = today.AddMilliseconds(schedule.At.TotalMilliseconds);
        var waitTime = runAt - now;
        if (waitTime.TotalMilliseconds < 0)
        {
            waitTime = waitTime.Add(TimeSpan.FromHours(24));
            runAt = now.AddMilliseconds(waitTime.TotalMilliseconds);
        }

        _logger.LogInformation(
            "Scheduling Task: Name: {TaskName}, Now: {Now}, Today: {Today}, Run At: {RunAt}, Wait Time: {WaitTime}",
            blockyTask.Name, now, today, runAt, waitTime);

        Task.Run(async () =>
        {
            do
            {
                await Task.Delay(waitTime, cancellationToken);
                await blockyTask.RunAsync(cancellationToken);
            } while (schedule.Repeat);
        }, cancellationToken);
    }
}