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

    public async Task<Torrent> GetTorrent(string hash)
    {
        try
        {
            var torrentInfo =  await client.GetTorrentListAsync();
            var torrent = torrentInfo.FirstOrDefault(t => t.Hash == hash);
            return new Torrent
            {
                Name = torrent.Name,
                Category = torrent.Category,
                Hash =  torrent.Hash,
                Progress = (int)(torrent.Progress *100),
                Status = torrent.State
            };
        }
        catch (Exception e)
        {
            log.Error(e, "Error deleting torrent");
            throw;
        }
    }

    public async Task<List<Category>> GetCategories()
    {
        try
        {
            var categoryList = new List<Category>();
            var categoriesInfo =  await client.GetCategoriesAsync();
            foreach (var categoryInfo in categoriesInfo)
            {
                categoryList.Add(categoryInfo.Value);
            }

            return categoryList;

        }
        catch (Exception e)
        {
            log.Error(e, "Error getting categories");
            throw;
        }
    }

    public async Task SetCategory(string category, string hash)
    {
        try
        {
            await client.SetTorrentCategoryAsync(hash, category);
        }
        catch (Exception e)
        {
            log.Error(e, "Error setting category");
            throw;
        }
    }

    public async Task DeleteTorrent(string hash)
    {
        try
        {
            await client.DeleteAsync(hash, true);
        }
        catch (Exception e)
        {
            log.Error(e, "Error deleting torrent");
            throw;
        }
    }

    public async Task ResumeTorrent(string hash)
    {
        try
        {
            await client.ResumeAsync(hash);
        }
        catch (Exception e)
        {
            log.Error(e, "Error resuming torrent");
            throw;
        }
    }

    public async Task PauseTorrent(string hash)
    {
        try
        {
            await client.PauseAsync(hash);
        }
        catch (Exception e)
        {
            log.Error(e, "Error pausing torrent");
            throw;
        }
    }
}