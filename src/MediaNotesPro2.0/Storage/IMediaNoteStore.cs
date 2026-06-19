using MediaNotesPro.Models;
namespace MediaNotesPro.Storage;
public interface IMediaNoteStore { Task<MediaNote> GetAsync(Guid u,Guid i,CancellationToken ct); Task<MediaNote> SaveAsync(Guid u,Guid i,string t,CancellationToken ct); }
