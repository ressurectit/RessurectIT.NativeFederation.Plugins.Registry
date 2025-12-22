namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Extensions;

/// <summary>
/// Class storing remotes extensions
/// </summary>
internal static class RemotesExtensions
{
    extension(string str)
    {
        /// <summary>
        /// Converts provided string to remote url
        /// </summary>
        /// <returns>Remote url</returns>
        public string ToRemoteUrl()
        {
            return $"{str}{(str.EndsWith('/') ? string.Empty : '/')}remoteEntry.json";
        }

        /// <summary>
        /// Converts provided string to version url
        /// </summary>
        /// <returns>Version url</returns>
        public string ToVersionUrl()
        {
            return $"{str}{(str.EndsWith('/') ? string.Empty : '/')}version.json";
        }
    }
}