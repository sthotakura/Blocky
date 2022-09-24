namespace BlockyApi.Core;

public interface IHostsFileReaderWriter
{
    Task<IReadOnlyCollection<IHostsFileLine>> ReadLinesAsync();

    event EventHandler<IReadOnlyCollection<IHostsFileLine>> LinesChanged;

    Task WriteLinesAsync(IEnumerable<IHostsFileLine> lines);
}