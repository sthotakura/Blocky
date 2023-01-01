using BlockyApi.Core;
using BlockyApi.Data;
using BlockyApi.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.WindowsServices;

var builderOptions = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(builderOptions);
builder.Host.UseWindowsService();
builder.Logging.AddLog4Net();

var services = builder.Services;
services.AddDataProtection();
services.AddCors(options =>
{
    options.AddPolicy(options.DefaultPolicyName,
        policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

services
    .AddHostedService<BlockyTasksHostedService>()
    .AddSingleton<IEncryptor, Encryptor>()
    .AddSingleton<IPasswordManager, PasswordManager>()
    .AddSingleton<IHostsFileLineParser, HostsFileLineParser>()
    .AddSingleton<IHostsFileEditorSettings, HostsFileEditorSettings>()
    .AddSingleton<IHostsFileReaderWriter, HostsFileReaderWriter>()
    .AddSingleton<IFlushDns, DnsFlusher>()
    .AddSingleton<IDateTimeService, DateTimeService>()
    .AddSingleton<PauseBlocking>()
    .AddSingleton<ResumeBlocking>()
    .AddSingleton<IBlockyTaskScheduler, BlockyTaskScheduler>()
    .AddSingleton<IBlockyService, BlockyService>();

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Blocky API!");

app.MapGet("/status", (IPasswordManager passwordManager) => new { hasSecret = passwordManager.HasPassword() });

app.MapPost("/secret", (IPasswordManager passwordManager, [FromBody] SecretInputModel secret) =>
{
    if (passwordManager.HasPassword())
    {
        return Results.BadRequest("A secret has already been set");
    }

    if (string.IsNullOrWhiteSpace(secret.Secret))
    {
        return Results.BadRequest("An empty secret provided.");
    }

    passwordManager.SetPassword(secret.Secret);

    return Results.Ok();
});

app.MapGet("/blocked-list", async (IBlockyService service) =>
{
    var blocked = await service.GetBlockedListAsync();
    return Results.Ok(blocked);
});

app.MapPost("/block", async (IBlockyService service, [FromBody] BlockInputModel input) =>
{
    if (string.IsNullOrWhiteSpace(input.Host))
    {
        return Results.BadRequest("host cannot be empty.");
    }

    var result = await service.BlockAsync(input.Host);
    return result ? Results.Ok() : Results.Problem($"could not block {input.Host}");
});

app.MapPost("/unblock",
    async (IPasswordManager passwordManager, IBlockyService service, [FromBody] UnblockInputModel input) =>
    {
        if (string.IsNullOrWhiteSpace(input.Host))
        {
            return Results.BadRequest("host cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(input.Secret))
        {
            return Results.BadRequest("secret cannot be empty.");
        }

        if (!passwordManager.IsPassword(input.Secret))
        {
            return Results.Unauthorized();
        }

        var result = await service.UnblockAsync(input.Host);
        return result ? Results.Ok() : Results.Problem($"could not unblock {input.Host}");
    });


await app.RunAsync();