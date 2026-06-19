using MediaBrowser.Controller.Plugins;
using MediaNotesPro.Storage;
using Microsoft.Extensions.DependencyInjection;
namespace MediaNotesPro;
public sealed class PluginServiceRegistrator:IPluginServiceRegistrator { public void RegisterServices(IServiceCollection s)=>s.AddSingleton<IMediaNoteStore,JsonMediaNoteStore>(); }
