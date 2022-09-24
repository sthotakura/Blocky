namespace BlockyApi.Core;

public interface IBlockyService
{
    Task<IReadOnlyCollection<string>> GetBlockedListAsync();
    
    Task<IBlockyService> BlockAsync(string hostName, string ipAddress = "127.0.0.1");

    Task<IBlockyService> UnblockAsync(string hostName);
}