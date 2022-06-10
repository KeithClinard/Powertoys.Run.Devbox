using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ManagedCommon;
using Powertoys.Run.Devbox.PluginCore;
using Powertoys.Run.Devbox.PluginCore.Models;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.VsCode
{
  public class Main : BasePlugin
  {
    public override string Name => "Devbox Settings";
    public override string Description => "Configure Devbox Plugins";
    public override List<Result> OnQuery(Query query)
    {

      List<Result> list = new List<Result>();
      if (query.Search.Length == 0)
      {
        list.Add(new Result
        {
          Title = $"apiToken {settings.ApiToken}",
          SubTitle = $"Set Github API Token - (Currently: {settings.ApiToken})",
          IcoPath = IconPath
        });
        list.Add(new Result
        {
          Title = $"gitFolder {settings.GitFolder}",
          SubTitle = $"Set Git Folder - (Currently: {settings.GitFolder})",
          IcoPath = IconPath
        });
        list.Add(new Result
        {
          Title = $"wslGitFolder {settings.WslGitFolder}",
          SubTitle = $"Set WSL Git Folder - (Currently: {settings.WslGitFolder})",
          IcoPath = IconPath
        });
        list.Add(new Result
        {
          Title = $"wslDistroName {settings.WslDistroName}",
          SubTitle = $"Set WSL Distro Name - (Currently: {settings.WslDistroName})",
          IcoPath = IconPath
        });
        return list;
      }

      string[] searchStrings = query.Search.Split(' ');

      if ("apiToken".Equals(searchStrings[0]))
      {
        string apiToken = "";
        if (searchStrings.Length > 1)
        {
          apiToken = searchStrings[1];
        }
        list.Add(new Result
        {
          Title = $"Set Github API Token to \"{apiToken}\"",
          SubTitle = $"Currently: \"{settings.ApiToken}\"",
          Action = (e) =>
          {
            settings.ApiToken = apiToken;
            storage.Save();
            return true;
          },
          IcoPath = IconPath
        });
        return list;
      }

      if ("gitFolder".Equals(searchStrings[0]))
      {
        string gitFolder = "";
        if (searchStrings.Length > 1)
        {
          gitFolder = searchStrings[1];
        }
        list.Add(new Result
        {
          Title = $"Set git folder to \"{gitFolder}\"",
          SubTitle = $"Currently: \"{settings.GitFolder}\"",
          Action = (e) =>
          {
            settings.GitFolder = gitFolder;
            storage.Save();
            return true;
          },
          IcoPath = IconPath
        });
        return list;
      }

      if ("wslGitFolder".Equals(searchStrings[0]))
      {
        string gitFolder = "";
        if (searchStrings.Length > 1)
        {
          gitFolder = searchStrings[1];
        }
        list.Add(new Result
        {
          Title = $"Set WSL git folder to \"{gitFolder}\"",
          SubTitle = $"Currently: \"{settings.WslGitFolder}\"",
          Action = (e) =>
          {
            settings.WslGitFolder = gitFolder;
            storage.Save();
            return true;
          },
          IcoPath = IconPath
        });
        return list;
      }

      if ("wslDistroName".Equals(searchStrings[0]))
      {
        string distroName = "";
        if (searchStrings.Length > 1)
        {
          distroName = searchStrings[1];
        }
        list.Add(new Result
        {
          Title = $"Set WSL Distro Name to \"{distroName}\"",
          SubTitle = $"Currently: \"{settings.WslDistroName}\"",
          Action = (e) =>
          {
            settings.WslDistroName = distroName;
            storage.Save();
            return true;
          },
          IcoPath = IconPath
        });
        return list;
      }

      return list;
    }

  }

}
