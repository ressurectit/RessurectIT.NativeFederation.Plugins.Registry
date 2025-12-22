using System.Reflection;
using System.Text.Json;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto;
using RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Enums;
using PluginEventInfo = (RessurectIT.NativeFederation.Plugins.Registry.Web.App.Misc.Enums.PluginEvent, RessurectIT.NativeFederation.Plugins.Registry.Web.App.Dto.PluginInfo);

namespace RessurectIT.NativeFederation.Plugins.Registry.Web.App.Services;

/// <summary>
/// Service that stores currently registered plugins in registry
/// </summary>
public class PluginsRegistryService
{
    #region private fields

    /// <summary>
    /// Instance of 'lock'
    /// </summary>
    private readonly Lock _lockObj = new();

    /// <summary>
    /// Task promise used for waiting for 'events'
    /// </summary>
    private TaskCompletionSource<PluginEventInfo> _taskPromise = new();

    /// <summary>
    /// Current content of registry
    /// </summary>
    private readonly List<PluginInfo> _registry;

    /// <summary>
    /// Instance of logger
    /// </summary>
    private readonly ILogger<PluginsRegistryService> _logger;
    #endregion


    #region private properties

    /// <summary>
    /// Gets registry json path
    /// </summary>
    private string RegistryJsonPath
    {
        get
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string? directory = Path.GetDirectoryName(path);

            ArgumentNullException.ThrowIfNull(directory);

            return Path.Combine(directory, "registry.json");
        }
    }
    #endregion


    #region public properties

    /// <summary>
    /// Gets current content of registry
    /// </summary>
    public List<PluginInfo> Registry => _registry;
    #endregion


    #region constructors
    
    /// <summary>
    /// Creates instance of <see cref="PluginsRegistryService"/>
    /// </summary>
    /// <param name="logger">Instance of logger</param>
    public PluginsRegistryService(ILogger<PluginsRegistryService> logger)
    {
        _logger = logger;
        _registry = ReadRegistry();
    }
    #endregion


    #region public methods

    /// <summary>
    /// Adds plugin info to registry
    /// </summary>
    /// <param name="pluginInfo">Plugin info to be added</param>
    /// <returns>Task for async call</returns>
    public void AddPluginInfo(PluginInfo pluginInfo)
    {
        _logger.LogInformation("Registering new plugin {@plugin}", pluginInfo);

        lock(_lockObj)
        {
            TaskCompletionSource<PluginEventInfo> taskPromise = _taskPromise;
            _taskPromise = new();
            _registry.Add(pluginInfo);
            WriteRegistry();
            taskPromise.SetResult((PluginEvent.Add, pluginInfo));
            
        }
    }

    /// <summary>
    /// Removes plugin info from registry
    /// </summary>
    /// <param name="pluginInfo">Plugin info to be removed</param>
    /// <returns>Task for async call</returns>
    public void RemovePluginInfo(PluginInfo pluginInfo)
    {
        _logger.LogInformation("Removing existing plugin {@plugin}", pluginInfo);

        lock(_lockObj)
        {
            TaskCompletionSource<PluginEventInfo> taskPromise = _taskPromise;
            _taskPromise = new();
            _registry.Remove(pluginInfo);
            WriteRegistry();
            taskPromise.SetResult((PluginEvent.Remove, pluginInfo));
        }
    }

    /// <summary>
    /// Waits for plugin events
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> that is used for canceling running request</param>
    /// <returns>Task with <see cref="PluginEventInfo"/></returns>
    public async Task<PluginEventInfo> WaitForPluginEvent(CancellationToken cancellationToken)
    {
        using (cancellationToken.Register(() => _taskPromise.TrySetCanceled())) 
        {
            return await _taskPromise.Task;
        } 
    }
    #endregion


    #region private methods

    /// <summary>
    /// Writes registry content to permanent store
    /// </summary>
    private void WriteRegistry()
    {
        if(!File.Exists(RegistryJsonPath))
        {
            _logger.LogInformation("File 'registry.json' does not exists, creating new one.");
        }
        else
        {
            _logger.LogInformation("File 'registry.json' exists, overwriting it.");
        }

        File.WriteAllText(RegistryJsonPath, JsonSerializer.Serialize(_registry));
    }

    /// <summary>
    /// Reads stored registry content
    /// </summary>
    /// <returns>List of loaded registry</returns>
    private List<PluginInfo> ReadRegistry()
    {
        if(!File.Exists(RegistryJsonPath))
        {
            _logger.LogInformation("File 'registry.json' does not exists!");

            return [];
        }

        try
        {
            string registryJsonText = File.ReadAllText(RegistryJsonPath);

            return JsonSerializer.Deserialize<List<PluginInfo>>(registryJsonText) ?? [];
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Failed to load 'registry.json'!");

            return [];
        }
    }
    #endregion
}
