using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ManagedCommon;
using Powertoys.Run.Devbox.Core;
using Powertoys.Run.Devbox.Core.Models;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.VsCode
{
  public class Main : CorePlugin
  {
    public override string Name => "VsCode";
    public override string Description => "Open repo in VsCode";
    public override List<Result> OnQuery(Query query)
    {
      List<Result> list = new List<Result>();

      if (query.Search.Length == 0)
      {
        list.Add(new Result
        {
          Title = "Open VSCode",
          SubTitle = "...or keep typing to search for repositories",
          Action = (e) =>
          {
            OpenVSCode("", false);
            return true;
          },
          IcoPath = IconPath
        });
        return list;
      }
      var searchString = query.Search;
      string[] splitQuery = searchString.Split(' ');
      if (splitQuery.Length > 1)
      {
        searchString = string.Join("*", splitQuery);
      }
      string[] wslResults = new string[0];
      try
      {
        if (!string.IsNullOrEmpty(settings.WslDistroName))
        {
          wslResults = Directory.GetDirectories($"\\\\wsl$\\{settings.WslDistroName}{settings.WslGitFolder}", $"*{searchString}*", SearchOption.TopDirectoryOnly);
        }
      }
      catch (Exception e)
      {
        list.Add(new Result
        {
          Title = "Devbox Plugin",
          SubTitle = "Error during WSL query: " + e.Message,
          IcoPath = IconPath
        });
      }
      string[] localResults = new string[0];
      try
      {
        localResults = Directory.GetDirectories(settings.GitFolder, $"*{searchString}*", SearchOption.TopDirectoryOnly);
      }
      catch (Exception e)
      {
        list.Add(new Result
        {
          Title = "Devbox Plugin",
          SubTitle = "Error during Windows query: " + e.Message,
          IcoPath = IconPath
        });
      }
      if (wslResults.Length > 0 || localResults.Length > 0)
      {
        foreach (string result in wslResults)
        {
          list.Add(new Result
          {
            Title = Path.GetFileName(result),
            SubTitle = "WSL",
            IcoPath = IconPath,
            Action = (e) =>
            {
              OpenVSCode(result, true);
              return true;
            }
          });
        }
        foreach (string result in localResults)
        {
          list.Add(new Result
          {
            Title = Path.GetFileName(result),
            SubTitle = "Windows",
            IcoPath = IconPath,
            Action = (e) =>
            {
              OpenVSCode(result, false);
              return true;
            }
          });
        }
      }
      else
      {
        list.Add(new Result
        {
          Title = "No Results Found",
          IcoPath = IconPath
        });
      }

      return list;
    }

    private void OpenVSCode(string folder, bool useWsl)
    {

      string command = $"code {folder}";
      if (useWsl)
      {
        string wslFolder = folder.Replace($"\\\\wsl$\\{settings.WslDistroName}", "");
        wslFolder = wslFolder.Replace("\\", "/");
        command = $"code --folder-uri vscode-remote://wsl+{settings.WslDistroName}{wslFolder}";
      }

      ProcessStartInfo info;
      var arguments = $"/c \"{command}\"";
      info = new ProcessStartInfo
      {
        FileName = "cmd.exe",
        Arguments = arguments,
        UseShellExecute = true,
        WindowStyle = ProcessWindowStyle.Hidden
      };

      Process.Start(info);
    }
  }

}
