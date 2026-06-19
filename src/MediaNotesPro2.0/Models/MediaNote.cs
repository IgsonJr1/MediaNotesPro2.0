namespace MediaNotesPro.Models;
public sealed record MediaNote(Guid UserId,Guid ItemId,string NotesText,DateTimeOffset LastModified);
public sealed record SaveMediaNoteRequest(string? NotesText);
