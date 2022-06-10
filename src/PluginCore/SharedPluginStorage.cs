using System.IO;
using Powertoys.Run.Devbox.PluginCore.Models;
using Wox.Infrastructure;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.PluginCore;

public class SharedPluginStorage : JsonStorage<SettingsModel>
{
  private readonly IPublicAPI _contextApi;
  public static readonly string sharedPluginName = "Powertoys.Run.Devbox";
  public SharedPluginStorage(IPublicAPI contextApi)
  {
    _contextApi = contextApi;
    var dataType = typeof(SettingsModel);
    DirectoryPath = Path.Combine(Constant.DataDirectory, DirectoryName, Constant.Plugins, sharedPluginName);
    Helper.ValidateDirectory(DirectoryPath);

    FilePath = Path.Combine(DirectoryPath, $"{dataType.Name}{FileSuffix}");
  }

  public new void Save()
  {
    base.Save();
    _contextApi.ReloadAllPluginData();
  }
}
