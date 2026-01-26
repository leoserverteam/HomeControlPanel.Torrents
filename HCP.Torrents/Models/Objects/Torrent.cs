using QBittorrent.Client;

namespace HS.Server.Models.Objects;

public class Torrent
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public int Progress { get; set; }
    public string Category { get; set; }
    
    public TorrentState Status { get; set; }
    
    public override string ToString()
    {
        return $"{Name}\nСтатус - {Status}\nПрогресс загрузки - {Progress}%\nКатегория:{Category}";
    }
}