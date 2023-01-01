namespace BlockyApi.Tasks;

public class BlockyTasksHostedService : IHostedService
{
    readonly IBlockyTaskScheduler _taskScheduler;
    readonly PauseBlocking _pausingTask;
    readonly ResumeBlocking _unPausingTask;

    public BlockyTasksHostedService(IBlockyTaskScheduler taskScheduler, PauseBlocking pausingTask,
        ResumeBlocking unPausingTask)
    {
        _taskScheduler = taskScheduler;
        _pausingTask = pausingTask;
        _unPausingTask = unPausingTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _taskScheduler.Schedule(_pausingTask, BlockyTaskSchedules.PauseSchedule, CancellationToken.None);
        _taskScheduler.Schedule(_unPausingTask, BlockyTaskSchedules.UnPauseSchedule, CancellationToken.None);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}