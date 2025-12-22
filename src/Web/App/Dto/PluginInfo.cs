namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;

/// <summary>
/// Information about loaded plugin
/// </summary>
/// <param name="Name">Name of loaded plugin</param>
/// <param name="Version">Version of plugin</param>
/// <param name="PublicUrl">URL that is used for accessing plugin publicly</param>
/// <param name="InternalUrl">URL that is used for accessing plugin internally</param>
public record PluginInfo(string Name, string Version, string PublicUrl, string InternalUrl);
