using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Powertoys.Run.Devbox.PluginCore;
using Powertoys.Run.Devbox.PluginCore.Models;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.Ember;

public class Main : BasePlugin
{
  private static List<EmberImportObject> _imports = null;

  public override string Name => "Ember";
  public override string Description => "Search Ember imports and copy to clipboard";
  public override List<Result> OnQuery(Query query)
  {
    var list = new List<Result>();

    if (query.Search.Length == 0)
    {
      list.Add(new Result
      {
        Title = "Lookup Ember Import",
        SubTitle = "Lookup Ember import strings and copy to clipboard",
        IcoPath = IconPath
      });
      return list;
    }
    var results = _imports.FindAll(result =>
    {
      var found = true;
      var searchStrings = query.Search.Split(' ');
      foreach (var searchSegment in searchStrings)
      {
        found = found && result.global.ToLower().Contains(searchSegment.ToLower());
      }
      return found;
    });

    if (results.Count > 0)
    {
      foreach (var item in results)
      {
        var module = item.module;
        var export = item.export;
        var localName = item.localName;
        if (item.deprecated && item.replacement != null)
        {
          module = item.replacement.module;
          export = item.replacement.export;
          if (export.Equals("default"))
          {
            var nonDeprecatedImport = _imports.Find(import =>
            {
              return import.module.Equals(module) && import.export.Equals(export);
            });
            localName = nonDeprecatedImport.localName;
          }
        }

        var importText = export.Equals("default") ? localName : "{ " + export + " }";
        var clipboardText = "import " + importText + " from '" + module + "';";
        list.Add(new Result
        {
          Title = item.global,
          SubTitle = clipboardText,
          IcoPath = IconPath,
          Action = (e) =>
          {
            WindowsClipboard.SetText(clipboardText);
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

  public override void OnReloadData()
  {
    using var stream = new StreamReader(Context.CurrentPluginMetadata.PluginDirectory + "\\mappings.json");
    var json = stream.ReadToEnd();
    _imports = JsonSerializer.Deserialize<List<EmberImportObject>>(json);
  }
}
