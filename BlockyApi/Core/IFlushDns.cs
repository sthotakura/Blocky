namespace BlockyApi.Core;

public interface IFlushDns
{
    Task<bool> FlushAsync();
}