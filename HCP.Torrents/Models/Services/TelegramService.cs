using QBittorrent.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using Vostok.Logging.Abstractions;

namespace HS.Server.Models.Services;

public class TelegramService (ITelegramBotClient botClient, ILog log, TorrentService torrentService, IWebHostEnvironment env, TelegramBtnBuilder btn)
{
    public async Task AddTorrentFile(Message message)
    {
        try
        {
            var torrent = await botClient.GetFile(message.Document.FileId);
            var uploadsFolder = Path.Combine(env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            uploadsFolder = Path.Combine(uploadsFolder, message.Document.FileName);
            using (Stream saveStream = File.OpenWrite(uploadsFolder))
            {
                await botClient.DownloadFile(torrent.FilePath, saveStream);
                log.Info("torrent downloaded");
            }

            await torrentService.AddTorrent(uploadsFolder);
            await botClient.SendMessage(message.Chat.Id, "Торрент файл успешно прочитан");
        }
        catch (Exception e)
        {
            log.Error(e);
            throw;
        }
    }

    public async Task AddTorrentMagnet(Message message)
    {
        try
        {
            await torrentService.AddMagnet(message.Text);
            await botClient.SendMessage(message.Chat.Id, "Magnet ссылка успешно прочитана");
        }
        catch (Exception e)
        {
            log.Error(e);
            throw;
        }
    }

    public async Task GetTorrents(CallbackQuery query)
    {
        try
        {
            var torrents = await torrentService.GetTorrents();
            string sendingString = "Список Торрентов\n\n";
            if (torrents.Count > 0)
            {
                foreach (var torrent in torrents)
                {
                    sendingString +=
                        $"{torrent}\n\n\n";
                }
                await botClient.DeleteMessage(query.Message.Chat.Id, query.Message.MessageId);
                await botClient.SendMessage(query.Message.Chat.Id, sendingString, replyMarkup: await btn.ControlMenu(torrents));
            }
            else
            {
                await botClient.DeleteMessage(query.Message.Chat.Id, query.Message.MessageId);
                await botClient.SendMessage(query.Message.Chat.Id, "Ни одного торрента не добавлено", replyMarkup: btn.MainMenu());
            }
        }
        catch (Exception e)
        {
            log.Error(e);
            throw;
        }
    }

    public async Task EditTorrent(CallbackQuery query)
    {
        try
        {
            var hash = query.Data.Substring("edit-".Length);
            var torrent = await torrentService.GetTorrent(hash);
            await botClient.EditMessageText(query.Message.Chat.Id, query.Message.MessageId, torrent.ToString());
            await botClient.EditMessageReplyMarkup(query.Message.Chat.Id, query.Message.MessageId,  await btn.EditMenu(torrent, await torrentService.GetCategories()));
        }
        catch (Exception e)
        {
            log.Error(e);
            throw;
        }
    }

    public async Task SetCategory(CallbackQuery query)
    {
        var parts = query.Data.Split(new []{"-setcat-"}, StringSplitOptions.None);
        var hash = parts[0];
        var category = parts[1];
        await torrentService.SetCategory(category, hash);
        var torrent = await torrentService.GetTorrent(hash);
        await botClient.EditMessageText(query.Message.Chat.Id, query.Message.MessageId, torrent.ToString());
        await botClient.EditMessageReplyMarkup(query.Message.Chat.Id, query.Message.MessageId,  await btn.EditMenu(torrent, await torrentService.GetCategories()));
    }

    public async Task Delete(CallbackQuery query)
    {
        var hash = query.Data.Substring("delete-".Length);
        await torrentService.DeleteTorrent(hash);
        await botClient.EditMessageText(query.Message.Chat.Id, query.Message.MessageId, "Торрент удален");
        await botClient.EditMessageReplyMarkup(query.Message.Chat.Id, query.Message.MessageId, btn.MainMenu());
    }

    public async Task AddedTorrentNote(string hash)
    {
        var torrent = await torrentService.GetTorrent(hash);
        await botClient.SendMessage(Environment.GetEnvironmentVariable("ADMIN"),
        $"Добавлен торрент\n{torrent}", replyMarkup:await btn.EditMenu(torrent, await torrentService.GetCategories()));
    }

    public async Task CompletedTorrentNote(string hash)
    {
        var torrent = await torrentService.GetTorrent(hash);
        await botClient.SendMessage(Environment.GetEnvironmentVariable("ADMIN"), $"Загружен торрент:\b{torrent.Name}");
    }
    
    
}