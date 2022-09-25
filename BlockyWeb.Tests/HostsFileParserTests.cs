using BlockyApi.Core;

namespace BlockyWeb.Tests;

public class HostsFileParserTests
{
    [Theory]
    [InlineData(null, true, false, null, null, false)]
    [InlineData("", true, false, null, null, false)]
    [InlineData("# A simple comment", false, true, null, null, false)]
    [InlineData("# 127.0.0.1 twitter.com", false, true, null, null, false)]
    [InlineData("            # 127.0.0.1 twitter.com", false, true, null, null, false)]
    [InlineData("127.0.0.1 twitter.com", false, false, "twitter.com", "127.0.0.1", false)]
    [InlineData("127.0.0.1 twitter.com   # twitter.com", false, false, "twitter.com", "127.0.0.1", false)]
    [InlineData("            127.0.0.1 twitter.com    # twitter.com", false, false, "twitter.com", "127.0.0.1", false)]
    [InlineData("        127.0.0.1      twitter.com           # twitter.com", false, false, "twitter.com", "127.0.0.1", false)]
    [InlineData("        127.0.0.1      twitter.com           # Added By Blocky", false, false, "twitter.com", "127.0.0.1", true)]
    public void Parse_Test(string line, bool isEmpty, bool isComment, string host, string ipAddress, bool isAddedByBlocky)
    {
        var parsed = new HostsFileLineParser().Parse(line);
        Assert.Equal(line, parsed.Line);
        Assert.Equal(isEmpty, parsed.IsEmpty);
        Assert.Equal(isComment, parsed.IsComment);
        Assert.Equal(host, parsed.Host);
        Assert.Equal(ipAddress, parsed.IpAddress);
        Assert.Equal(isAddedByBlocky, parsed.IsAddedByBlocky);
    }
}