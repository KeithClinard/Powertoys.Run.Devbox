using System;
using System.Collections.Generic;
using System.IO;
using ManagedCommon;
using Powertoys.Run.Devbox.Core;
using Powertoys.Run.Devbox.Core.Models;
using Wox.Infrastructure;
using Wox.Infrastructure.Storage;
using Wox.Infrastructure.UserSettings;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.Core
{
  public class SharedPluginStorage : JsonStorage<SettingsModel>
  {
    private IPublicAPI _contextApi;
    public static readonly string sharedPluginName = "Powertoys.Run.Devbox";
    public SharedPluginStorage(IPublicAPI contextApi)
    {
      _contextApi = contextApi;
      var dataType = typeof(SettingsModel);
      DirectoryPath = Path.Combine(Constant.DataDirectory, DirectoryName, Constant.Plugins, sharedPluginName);
      Helper.ValidateDirectory(DirectoryPath);

      FilePath = Path.Combine(DirectoryPath, $"{dataType.Name}{FileSuffix}");
    }

    new void Save()
    {
      base.Save();
      _contextApi.ReloadAllPluginData();
    }
  }
}
