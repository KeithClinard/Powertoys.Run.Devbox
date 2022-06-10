using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using ManagedCommon;
using Powertoys.Run.Devbox.PluginCore;
using Powertoys.Run.Devbox.PluginCore.Models;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.Ember
{
  public class Main : BasePlugin
  {
    private static List<EmberImportObject> _imports = null;

    public override string Name => "Ember";
    public override string Description => "Search Ember imports and copy to clipboard";
    public override List<Result> OnQuery(Query query)
    {
      List<Result> list = new List<Result>();

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
      List<EmberImportObject> results = _imports.FindAll(result =>
      {
        bool found = true;
        string[] searchStrings = query.Search.Split(' ');
        foreach (string searchSegment in searchStrings)
        {
          found = found && result.global.ToLower().Contains(searchSegment.ToLower());
        }
        return found;
      });

      if (results.Count > 0)
      {
        foreach (EmberImportObject item in results)
        {
          string module = item.module;
          string export = item.export;
          string localName = item.localName;
          if (item.deprecated && item.replacement != null)
          {
            module = item.replacement.module;
            export = item.replacement.export;
            if (export.Equals("default"))
            {
              EmberImportObject nonDeprecatedImport = _imports.Find(import =>
              {
                return import.module.Equals(module) && import.export.Equals(export);
              });
              localName = nonDeprecatedImport.localName;
            }
          }

          string importText = export.Equals("default") ? localName : "{ " + export + " }";
          string clipboardText = "import " + importText + " from '" + module + "';";
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
      using (StreamReader stream = new StreamReader(Context.CurrentPluginMetadata.PluginDirectory + "\\mappings.json"))
      {
        var json = stream.ReadToEnd();
        _imports = JsonSerializer.Deserialize<List<EmberImportObject>>(json);
      }
    }
  }
}
