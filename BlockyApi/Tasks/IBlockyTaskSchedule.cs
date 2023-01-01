namespace BlockyApi.Tasks;

public interface IBlockyTaskSchedule
{
    TimeSpan At { get; }

    bool Repeat { get; }
}