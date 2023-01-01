namespace BlockyApi.Tasks;

public interface IBlockyTask
{
    string Name { get; }

    Task RunAsync(CancellationToken cancellationToken);
}