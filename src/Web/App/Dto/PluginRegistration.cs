namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;

/// <summary>
/// Information about plugin registration
/// </summary>
/// <param name="PublicUrl">URL that is used for accessing plugin publicly</param>
/// <param name="InternalUrl">URL that is used for accessing plugin internally</param>
public record PluginRegistration(string PublicUrl, string InternalUrl);
