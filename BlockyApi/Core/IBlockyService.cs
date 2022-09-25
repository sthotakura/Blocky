namespace BlockyApi.Core;

public interface IBlockyService
{
    Task<IReadOnlyCollection<string>> GetBlockedListAsync();
    
    Task<bool> BlockAsync(string hostName, string ipAddress = "127.0.0.1");

    Task<bool> UnblockAsync(string hostName);
}