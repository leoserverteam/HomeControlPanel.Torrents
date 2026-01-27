using HS.Server.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Vostok.Logging.Abstractions;

namespace HCP.Torrents.Controllers;
[ApiController]
public class TorrentController (ILog log, TelegramService telegramService) : ControllerBase
{
    [HttpPost("api/start")]
    public async Task<ActionResult> StartNotify()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var hash = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(hash)) await telegramService.AddedTorrentNote(hash);
            return Ok();
        }
        catch (Exception e)
        {
           log.Error(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("api/complete")]
    public async Task<ActionResult> CompleteNotify()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var hash = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(hash)) await telegramService.CompletedTorrentNote(hash);
            return Ok();
        }
        catch (Exception e)
        {
            log.Error(e);
            return StatusCode(500, e.Message);
        }
    }
}