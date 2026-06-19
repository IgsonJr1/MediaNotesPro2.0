using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using MediaNotesPro.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace MediaNotesPro;

public sealed class PluginServiceRegistrator : IPluginServiceRegistrator
{
    public void RegisterServices(
        IServiceCollection services,
        IServerApplicationHost applicationHost)
    {
        services.AddSingleton<IMediaNoteStore, JsonMediaNoteStore>();
    }
}
