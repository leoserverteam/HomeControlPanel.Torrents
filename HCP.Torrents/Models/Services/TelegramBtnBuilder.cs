using HS.Server.Models.Objects;
using QBittorrent.Client;
using Telegram.Bot.Types.ReplyMarkups;

namespace HS.Server.Models.Services;

public class TelegramBtnBuilder 
{
    public InlineKeyboardMarkup MainMenu()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Список торрентов", "torrents"), 
            },
        });
    }

    public async Task<InlineKeyboardMarkup> ControlMenu(List<Torrent> torrents)
    {
        List<InlineKeyboardButton[]> btns = new List<InlineKeyboardButton[]>();
        foreach (var torrent in torrents)
        {
            btns.Add(new []
            {
                InlineKeyboardButton.WithCallbackData(torrent.Name[..30]+"...", $"edit-{torrent.Hash}")
            });
        }
        return new InlineKeyboardMarkup(btns);
        
    }

    public async Task<InlineKeyboardMarkup> EditMenu(Torrent torrent, )
    {
        List<InlineKeyboardButton[]> btns = new List<InlineKeyboardButton[]>();
        if (torrent.Status == TorrentState.Downloading)
        {
            btns.Add(new []
            {
                InlineKeyboardButton.WithCallbackData("Остановить", $"stop-{torrent.Hash}")
            });
        }
        else if (torrent.Status == TorrentState.PausedDownload || torrent.Status == TorrentState.PausedUpload)
        {
            btns.Add(new []
            {
                InlineKeyboardButton.WithCallbackData("Запустить", $"start-{torrent.Hash}")
            });
        }
        else
        {
            btns.Add(new []
            {
                InlineKeyboardButton.WithCallbackData("Попробовать запустить", $"start-{torrent.Hash}")
            });
        }
    }
}