using System.Collections.Concurrent;
using System.Text.Json;
using MediaBrowser.Common.Configuration;
using MediaNotesPro.Models;
namespace MediaNotesPro.Storage;
public sealed class JsonMediaNoteStore:IMediaNoteStore
{
 static readonly JsonSerializerOptions Json=new(JsonSerializerDefaults.Web){WriteIndented=true};
 readonly string dir; readonly ConcurrentDictionary<Guid,SemaphoreSlim> locks=new();
 public JsonMediaNoteStore(IApplicationPaths p){dir=Path.Combine(p.DataPath,"MediaNotesPro","notes");Directory.CreateDirectory(dir);}
 public async Task<MediaNote> GetAsync(Guid u,Guid i,CancellationToken ct)
 {var g=locks.GetOrAdd(u,_=>new(1,1));await g.WaitAsync(ct);try{var a=await Read(u,ct);return a.GetValueOrDefault(i)??new(u,i,"",DateTimeOffset.MinValue);}finally{g.Release();}}
 public async Task<MediaNote> SaveAsync(Guid u,Guid i,string t,CancellationToken ct)
 {var g=locks.GetOrAdd(u,_=>new(1,1));await g.WaitAsync(ct);try{var a=await Read(u,ct);var n=new MediaNote(u,i,t,DateTimeOffset.UtcNow);a[i]=n;await Write(u,a,ct);return n;}finally{g.Release();}}
 async Task<Dictionary<Guid,MediaNote>> Read(Guid u,CancellationToken ct){var p=PathFor(u);if(!File.Exists(p))return new();await using var s=File.OpenRead(p);return await JsonSerializer.DeserializeAsync<Dictionary<Guid,MediaNote>>(s,Json,ct)??new();}
 async Task Write(Guid u,Dictionary<Guid,MediaNote>a,CancellationToken ct){var p=PathFor(u);var tmp=p+".tmp";await using(var s=new FileStream(tmp,FileMode.Create,FileAccess.Write,FileShare.None,81920,FileOptions.Asynchronous|FileOptions.WriteThrough)){await JsonSerializer.SerializeAsync(s,a,Json,ct);await s.FlushAsync(ct);}File.Move(tmp,p,true);}
 string PathFor(Guid u)=>Path.Combine(dir,u.ToString("N")+".json");
}
