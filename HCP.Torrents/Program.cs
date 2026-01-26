using HS.Server.Models.Services;
using QBittorrent.Client;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IQBittorrentClient2, QBittorrentClient>(_ =>
{
    var client = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =  HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return new QBittorrentClient(new Uri(Environment.GetEnvironmentVariable("TORRENTHOST")), ApiLevel.Auto, client  , true);
});
builder.Services.AddSingleton<ILog, ConsoleLog>();
builder.Services.AddSingleton<TorrentService>();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(_=> new TelegramBotClient(Environment.GetEnvironmentVariable("BOTAPI")));
builder.Services.AddSingleton<IUpdateHandler, UpdateHandler>();
builder.Services.AddHostedService<BotHostedService>();
builder.Services.AddSingleton<TelegramBtnBuilder>();
builder.Services.AddSingleton<TelegramService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();