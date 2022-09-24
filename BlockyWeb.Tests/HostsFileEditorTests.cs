using BlockyApi.Core;
using Moq;
using Xunit.Abstractions;

namespace BlockyWeb.Tests;

public class HostsFileEditorTests
{
    readonly ITestOutputHelper _outputHelper;

    public HostsFileEditorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Editor_Invokes_Reader_And_Writer_To_Load_And_Save_Lines_Add()
    {
        var readerWriter = new Mock<IHostsFileReaderWriter>(MockBehavior.Strict);
        readerWriter.Setup(r => r.ReadLinesAsync()).ReturnsAsync(new List<IHostsFileLine>().AsReadOnly());
        readerWriter.Setup(w => w.WriteLinesAsync(It.IsAny<IEnumerable<IHostsFileLine>>())).Returns(Task.CompletedTask);

        var editor = new BlockyService(new SimpleLogger<BlockyService>(_outputHelper), readerWriter.Object,
            new HostsFileLineParser());
        await editor.BlockAsync("test-host");

        readerWriter.Verify(r => r.ReadLinesAsync(), Times.Once);
        readerWriter.Verify(r => r.ReadLinesAsync(), Times.Once);
    }

    [Fact]
    public async Task Editor_Invokes_Reader_And_Writer_To_Load_And_Save_Lines_Remove()
    {
        var readerWriter = new Mock<IHostsFileReaderWriter>(MockBehavior.Strict);
        readerWriter.Setup(r => r.ReadLinesAsync()).ReturnsAsync(new List<IHostsFileLine>().AsReadOnly());
        readerWriter.Setup(w => w.WriteLinesAsync(It.IsAny<IEnumerable<IHostsFileLine>>())).Returns(Task.CompletedTask);

        var editor = new BlockyService(new SimpleLogger<BlockyService>(_outputHelper), readerWriter.Object,
            new HostsFileLineParser());
        await editor.UnblockAsync("test-host");

        readerWriter.Verify(r => r.ReadLinesAsync(), Times.Once);
        readerWriter.Verify(r => r.ReadLinesAsync(), Times.Once);
    }
}