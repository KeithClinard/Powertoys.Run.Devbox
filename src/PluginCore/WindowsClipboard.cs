using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Powertoys.Run.Devbox.PluginCore
{
  static class WindowsClipboard
  {
    public static string SetText(string val)
    {
      var cmd = $"echo {val} | clip";
      var escapedArgs = cmd.Replace("\"", "\\\"");
      string result = Run("cmd.exe", $"/c \"{escapedArgs}\"");
      return result;
    }

    private static string Run(string filename, string arguments)
    {
      var process = new Process()
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = filename,
          Arguments = arguments,
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = false,
        }
      };
      process.Start();
      string result = process.StandardOutput.ReadToEnd();
      process.WaitForExit();
      return result;
    }
  }

}
