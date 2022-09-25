using System.Diagnostics;

namespace BlockyApi.Core;

public sealed class DnsFlusher : IFlushDns
{
    public async Task<bool> FlushAsync()
    {
        var startInfo = new ProcessStartInfo("ipconfig", "/flushdns");
        var process = new Process
        {
            StartInfo = startInfo
        };
        var started = process.Start();
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }
}