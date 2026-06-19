using MediaBrowser.Model.Plugins;
namespace MediaNotesPro;
public sealed class PluginConfiguration : BasePluginConfiguration { public bool AutoSaveEnabled { get; set; } = true; public int AutoSaveDelayMilliseconds { get; set; } = 700; }
