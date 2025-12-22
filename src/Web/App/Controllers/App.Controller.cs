using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Constants;

namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Controllers;

/// <summary>
/// Controller for app information
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
   #region public methods - actions

   /// <summary>
   /// Gets version of application
   /// </summary>
   /// <returns>String version of app</returns>
   [AllowAnonymous]
   [EndpointGroupName("v1")]
   [EndpointSummary("GetVersion")]
   [EndpointName("GetVersion")]
   [EndpointDescription("Gets version of application")]
   [ProducesResponseType<string>(StatusCodes.Status200OK, ContentTypes.Text)]
   [HttpGet("version")]
   public ActionResult GetVersion() => Ok(DependencyContext.Default?.CompileLibraries.Single(itm => itm.Name == "RessurectIT.NativeFederation.Plugins.Registry.Web.App").Version ?? "");
   #endregion
}
