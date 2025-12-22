using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Extensions;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Services;

namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Controllers;

/// <summary>
/// Controller for managing registry
/// </summary>
/// <param name="registry">Instance of registry service</param>
/// <param name="logger">Instance of logger</param>
[AllowAnonymous]
[ApiController]
[Route("internalapi/registry")]
public class InternalRegistryController(PluginsRegistryService registry,
                                        ILogger<InternalRegistryController> logger) : ControllerBase
{
    #region public methods - actions

    /// <summary>
    /// Register Plugin
    /// </summary>
    /// <remarks>
    /// Registers new plugin and obtains all necessary data
    /// </remarks>
    /// <param name="plugin">Public url for accessing remote plugin</param>
    /// <param name="cancellationToken">Token used for cancelation of reading logs</param>
    /// <returns>Empty result</returns>
    [AllowAnonymous]
    [EndpointGroupName("v1")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("register")]
    public async ValueTask<ActionResult> RegisterPlugin([FromBody] PluginRegistration plugin, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new plugin {@plugin}", plugin);

        using HttpClient client = new();
        Uri versionUri = new(plugin.InternalUrl.ToVersionUrl());
        HttpResponseMessage versionResponse = await client.GetAsync(versionUri, cancellationToken);
        VersionJson? versionJson = await versionResponse.Content.ReadFromJsonAsync<VersionJson>(cancellationToken);

        if(versionJson is null)
        {
            logger.LogError("Failed to obtain version for plugin!");

            return BadRequest();
        }

        Uri remoteUrl = new(plugin.InternalUrl.ToRemoteUrl());
        HttpResponseMessage remoteResponse = await client.GetAsync(remoteUrl, cancellationToken);
        RemoteEntry? remoteJson = await remoteResponse.Content.ReadFromJsonAsync<RemoteEntry>(cancellationToken);

        if(remoteJson is null)
        {
            logger.LogError("Failed to obtain remote name for plugin!");

            return BadRequest();
        }

        registry.AddPluginInfo(new (remoteJson.Name, versionJson.Version, plugin.PublicUrl, plugin.InternalUrl));

        logger.LogInformation("Plugin was successfully registered {@plugin}", plugin);

        return NoContent();
    }

    /// <summary>
    /// Unregister Plugin
    /// </summary>
    /// <remarks>
    /// Unregisters existing plugin
    /// </remarks>
    /// <param name="plugin">Public url for accessing remote plugin</param>
    /// <param name="cancellationToken">Token used for cancelation of reading logs</param>
    /// <returns>Empty result</returns>
    [AllowAnonymous]
    [EndpointGroupName("v1")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("unregister")]
    public async ValueTask<ActionResult> UnregisterPlugin([FromBody] PluginRegistration plugin, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unregistering existing plugin {@plugin}", plugin);

        using HttpClient client = new();
        Uri versionUri = new(plugin.InternalUrl.ToVersionUrl());
        HttpResponseMessage versionResponse = await client.GetAsync(versionUri, cancellationToken);
        VersionJson? versionJson = await versionResponse.Content.ReadFromJsonAsync<VersionJson>(cancellationToken);

        if(versionJson is null)
        {
            logger.LogError("Failed to obtain version for plugin!");

            return BadRequest();
        }

        Uri remoteUrl = new(plugin.InternalUrl.ToRemoteUrl());
        HttpResponseMessage remoteResponse = await client.GetAsync(remoteUrl, cancellationToken);
        RemoteEntry? remoteJson = await remoteResponse.Content.ReadFromJsonAsync<RemoteEntry>(cancellationToken);

        if(remoteJson is null)
        {
            logger.LogError("Failed to obtain remote name for plugin!");

            return BadRequest();
        }

        registry.RemovePluginInfo(new (remoteJson.Name, versionJson.Version, plugin.PublicUrl, plugin.InternalUrl));

        logger.LogInformation("Plugin was successfully unregistered {@plugin}", plugin);

        return NoContent();
    }
    #endregion
}
