using BlockyApi.Core;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddDataProtection();

services
    .AddSingleton<IEncryptor, Encryptor>()
    .AddSingleton<IPasswordManager, PasswordManager>()
    .AddSingleton<IHostsFileLineParser, HostsFileLineParser>()
    .AddSingleton<IHostsFileEditorSettings, HostsFileEditorSettings>()
    .AddSingleton<IHostsFileReaderWriter, HostsFileReaderWriter>()
    .AddSingleton<IBlockyService, BlockyService>();

var app = builder.Build();

app.MapGet("/", () => "Blocky API!");

app.Run();