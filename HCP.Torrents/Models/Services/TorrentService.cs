using HS.Server.Models.Objects;
using QBittorrent.Client;
using Vostok.Logging.Abstractions;

namespace HS.Server.Models.Services;

public class TorrentService (IQBittorrentClient2 client, ILog log)
{
    public async Task AddMagnet(string magnetLink)
    {
        try
        {
            log.Info("new magnet");
           await client.AddTorrentsAsync(new AddTorrentsRequest(new List<Uri> { new Uri(magnetLink) }));
           log.Info("added magnet");
        }
        catch (Exception e)
        {
            log.Error(e, "Error adding magnet");
            throw;
        }
    }

    public async Task AddTorrent(string file)
    {
        try
        {
            await client.AddTorrentsAsync(new AddTorrentsRequest(new List<string> { file }));
            log.Info("added torrent to tracker");
            File.Delete(file);
            log.Info("file deleted");
        }
        catch (Exception e)
        {
            log.Error(e, "Error adding torrent");
            throw;
        }
    }

    public async Task<List<Torrent>> GetTorrents()
    {
        try
        {
            var torrentsInfo = await client.GetTorrentListAsync();
            var torrents = new List<Torrent>();
            foreach (var torrentInfo in torrentsInfo)
            {
                torrents.Add(new Torrent
                {
                    Hash = torrentInfo.Hash,
                    Name = torrentInfo.Name,
                    Category = torrentInfo.Category,
                    Progress = (int)(torrentInfo.Progress *100),
                    Status = torrentInfo.State
                });
            }

            return torrents;
        }
        catch (Exception e)
        {
            log.Error(e, "Error getting torrents");
            throw;
        }
    }

    public async Task<IReadOnlyDictionary<string, Category>> GetCategories()
    {
        try
        {
            return await client.GetCategoriesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}