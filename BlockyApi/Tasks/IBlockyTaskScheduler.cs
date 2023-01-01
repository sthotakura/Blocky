namespace BlockyApi.Tasks;

public interface IBlockyTaskScheduler
{
    void Schedule(IBlockyTask blockyTask, IBlockyTaskSchedule schedule, CancellationToken cancellationToken);
}