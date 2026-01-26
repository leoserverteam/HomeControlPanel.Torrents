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

            torrentService.AddTorrent(uploadsFolder);
            botClient.SendMessage(message.Chat.Id, "Торрент файл добавлен на сервер, загрузка началась");
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
            await botClient.SendMessage(message.Chat.Id, "Добавлен торрент по magnet ссылке, загрузка началась");
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
            var torrents = await torrentService.GetTorrents();
            var torrent = torrents.FirstOrDefault(t => t.Hash == hash);
            await botClient.EditMessageText(query.Message.Chat.Id, query.Message.MessageId, torrent.ToString());
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}