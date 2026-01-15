using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Constants;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Enums;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Extensions;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Services;

namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Controllers;

/// <summary>
/// Controller for obtaining registry data
/// </summary>
/// <param name="registry">Instance of registry service</param>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class RegistryController(PluginsRegistryService registry) : ControllerBase
{
    #region public methods - actions

    /// <summary>
    /// Get Plugins
    /// </summary>
    /// <remarks>
    /// Gets plugins SSE stream, available event types 'All', 'Add', 'Remove', 'Update'
    /// </remarks>
    /// <param name="cancellationToken">Token used for cancelation of reading logs</param>
    /// <returns>SSE stream with event types 'All', 'Add', 'Remove', 'Update'</returns>
    [AllowAnonymous]
    [EndpointGroupName("v1")]
    [ProducesResponseType<IEnumerable<RemoteData>>(StatusCodes.Status200OK, ContentTypes.Json)]
    [HttpGet("")]
    public IResult GetPlugins(CancellationToken cancellationToken)
    {
        async IAsyncEnumerable<SseItem<object>> ReadRegistry([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            yield return new SseItem<object>(registry.Registry.Select(itm => new RemoteData(itm.Name, itm.PublicUrl.ToRemoteUrl(), itm.Version)), nameof(EventType.All));

            while(!cancellationToken.IsCancellationRequested)
            {
                (PluginEvent pluginEvent, PluginInfo info) = await registry.WaitForPluginEvent(cancellationToken);

                yield return new SseItem<object>(new RemoteData(info.Name, info.PublicUrl.ToRemoteUrl(), info.Version),
                                                 pluginEvent.ToString());
            }
        }

        return TypedResults.ServerSentEvents(ReadRegistry(cancellationToken));
    }
    #endregion
}
