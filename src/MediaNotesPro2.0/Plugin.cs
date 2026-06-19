using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
namespace MediaNotesPro;
public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
 public Plugin(IApplicationPaths p, IXmlSerializer s) : base(p,s) {}
 public override string Name => "Media Notes Pro";
 public override Guid Id => Guid.Parse("9a44e327-1087-4b62-9c96-a54b203f20be");
 public IEnumerable<PluginPageInfo> GetPages()
 {
  var r=GetType().Namespace+".Web.";
  yield return P("MediaNotesPro",r+"configuration.html"); yield return P("MediaNotesPro.js",r+"media-notes-pro.js");
  yield return P("MediaNotesPro.css",r+"media-notes-pro.css"); yield return P("MediaNotesProMobile",r+"mobile.html");
 }
 private static PluginPageInfo P(string n,string r)=>new(){Name=n,EmbeddedResourcePath=r};
}
