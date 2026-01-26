using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Vostok.Logging.Abstractions;

namespace HS.Server.Models.Services;

public class BotHostedService (IUpdateHandler updateHandler, ILog log, ITelegramBotClient client) : IHostedService
{
    private readonly ReceiverOptions receiverOptions = new()
    {
        AllowedUpdates =
        [
            UpdateType.Message,
            UpdateType.CallbackQuery
        ]
    };
    private CancellationTokenSource botCts = new();
    private readonly BotCommand[] botCommands = new[]
    {
        new BotCommand { Command = "/start", Description = "Главное меню" },
    };


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            client.StartReceiving(updateHandler.HandleUpdateAsync, updateHandler.HandleErrorAsync, receiverOptions,
                botCts.Token);
            await client.SetMyCommands(botCommands);
            Task.Delay(-1);
        }
        catch (Exception e)
        {
            log.Error(e, "Error starting bot");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            log.Info("Telegram stopping");
            await botCts.CancelAsync();
        }
        catch (Exception e)
        {
            log.Error("Telegram stop Error", e.Message);
            throw;
        }

    }
}