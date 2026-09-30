namespace AspHomework.Services;

public interface ITimeService
{
    string GetCurrentTimestamp();
}

public class TimeService : ITimeService
{
    public string GetCurrentTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
}
