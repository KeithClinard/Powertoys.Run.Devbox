using System.Diagnostics;

namespace Powertoys.Run.Devbox.PluginCore;

internal static class WindowsClipboard
{
  public static string SetText(string val)
  {
    var cmd = $"echo {val} | clip";
    var escapedArgs = cmd.Replace("\"", "\\\"");
    var result = Run("cmd.exe", $"/c \"{escapedArgs}\"");
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
    _ = process.Start();
    var result = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    return result;
  }
}

