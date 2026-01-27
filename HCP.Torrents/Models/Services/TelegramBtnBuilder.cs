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
            if (torrent.Name.Length > 30)
            {
                btns.Add(new []
                {
                    InlineKeyboardButton.WithCallbackData(torrent.Name[..30]+"...", $"edit-{torrent.Hash}")
                });
            }
            else
            {
                btns.Add(new []
                {
                    InlineKeyboardButton.WithCallbackData(torrent.Name, $"edit-{torrent.Hash}")
                });
            }
        }
        return new InlineKeyboardMarkup(btns);
        
    }

    public async Task<InlineKeyboardMarkup> EditMenu(Torrent torrent, List<Category> categories )
    {
        List<InlineKeyboardButton[]> btns = new List<InlineKeyboardButton[]>();
        var subbtn = new List<InlineKeyboardButton>();
        foreach (var category in categories)
        {
            if (category.Name == torrent.Category)
            {
                subbtn.Add(InlineKeyboardButton.WithCallbackData($"✔️{category.Name}", $"{torrent.Hash}-setcat-{category.Name}"));
            }
            else
            {
                subbtn.Add(InlineKeyboardButton.WithCallbackData($"{category.Name}", $"{torrent.Hash}-setcat-{category.Name}"));
            }
        }
        btns.Add(subbtn.ToArray());
        btns.Add(new []
        {
            InlineKeyboardButton.WithCallbackData("Удалить вместе с файлами", $"delete-{torrent.Hash}")
        });
        btns.Add(new []
        {
            InlineKeyboardButton.WithCallbackData("Вернуться", $"torrents")
        });
        return new InlineKeyboardMarkup(btns);
    }
}