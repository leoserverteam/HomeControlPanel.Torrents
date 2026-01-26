using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Vostok.Logging.Abstractions;

namespace HS.Server.Models.Services;

public class UpdateHandler (ILog log, TelegramBtnBuilder btn, TelegramService telegramService) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    log.Info("new message");
                    if (update.Message.Type == MessageType.Document)
                    {
                        if (update.Message.Document.FileName.EndsWith(".torrent"))
                        {
                            telegramService.AddTorrentFile(update.Message);
                        }
                    }

                    if (update.Message.Text == "/start")
                    {
                        await botClient.SendMessage(update.Message.Chat.Id,
                            "Добро пожаловать в HomeControlPanel - Torrents", replyMarkup: btn.MainMenu());
                    }
                    var math = Regex.Match(update.Message.Text,
                        "magnet:\\?xt=urn:btih:[a-zA-Z0-9]{32,40}(?:&[a-zA-Z0-9%?=&-]+)*", RegexOptions.IgnoreCase);
                    if (math.Success)
                    {
                        telegramService.AddTorrentMagnet(update.Message);
                    }
                    break;
                case  UpdateType.CallbackQuery:
                    log.Info("new callback query");
                    if (update.CallbackQuery.Data == "torrents")
                    {
                        await telegramService.GetTorrents(update.CallbackQuery);
                    }
                    
                    break;
            }
        }
        catch (Exception e)
        {
            log.Error(e, $"Handle Update Error - {e.Message}");
            botClient.SendMessage(update.Message.Chat.Id, "Ошибка при попытке обработать твое сообщение");
            throw;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        log.Warn(exception, $"Telegram API Error - {exception.Message}");

    }
}