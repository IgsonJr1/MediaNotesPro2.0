using System.Security.Claims;
using System.Text;
using MediaNotesPro.Models;
using MediaNotesPro.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MediaNotesPro.Controllers;
[ApiController,Authorize,Route("MediaNotes"),Produces("application/json")]
public sealed class MediaNotesController:ControllerBase
{
 readonly IMediaNoteStore store; public MediaNotesController(IMediaNoteStore s)=>store=s;
 [HttpGet("{itemId:guid}")] public async Task<ActionResult<MediaNote>> Get(Guid itemId,CancellationToken ct)=>Ok(await store.GetAsync(UserId(),itemId,ct));
 [HttpPost("{itemId:guid}")] public async Task<ActionResult<MediaNote>> Save(Guid itemId,[FromBody]SaveMediaNoteRequest r,CancellationToken ct){var t=r.NotesText??"";if(t.Length>2_000_000)return BadRequest();return Ok(await store.SaveAsync(UserId(),itemId,t,ct));}
 [HttpGet("{itemId:guid}/Export"),Produces("text/plain")] public async Task<IActionResult> Export(Guid itemId,CancellationToken ct){var n=await store.GetAsync(UserId(),itemId,ct);return File(new UTF8Encoding(true).GetBytes(n.NotesText),"text/plain; charset=utf-8",$"media-notes-{itemId:N}.txt");}
 Guid UserId(){var v=User.FindFirstValue(ClaimTypes.NameIdentifier)??User.FindFirstValue("UserId")??User.FindFirstValue("sub");if(!Guid.TryParse(v,out var id))throw new UnauthorizedAccessException();return id;}
}
