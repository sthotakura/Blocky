namespace BlockyApi.Tasks;

public static class BlockyTaskSchedules
{
    class AnonymousTaskSchedule : IBlockyTaskSchedule
    {
        public TimeSpan At { get; private set; }

        public bool Repeat { get; private set; }

        internal static IBlockyTaskSchedule From(TimeSpan at, bool repeat = true)
        {
            return new AnonymousTaskSchedule
            {
                At = at,
                Repeat = repeat
            };
        }
    }

    public static readonly IBlockyTaskSchedule PauseSchedule =
        AnonymousTaskSchedule.From(TimeSpan.FromHours(17).Add(TimeSpan.FromMinutes(30)));
    
    public static readonly IBlockyTaskSchedule UnPauseSchedule =
        AnonymousTaskSchedule.From(TimeSpan.FromHours(9).Add(TimeSpan.FromMinutes(30)));
}