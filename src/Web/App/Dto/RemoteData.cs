namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;

/// <summary>
/// Information about remote
/// </summary>
/// <param name="Name">Name of remote</param>
/// <param name="Url">Url where remote can be found</param>
/// <param name="Version">Version of deployed remote</param>
public record RemoteData(string Name, string Url, string Version);