namespace BlockyApi.Core;

public interface IDateTimeService
{
    DateTime Now { get; }

    DateTime Today { get; }
}

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;
}