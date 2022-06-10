using System;
using System.Collections.Generic;
using ManagedCommon;
using Powertoys.Run.Devbox.Core.Models;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.Core
{
  public abstract class CorePlugin : IPlugin, IReloadable
  {

    private const string defaultGitFolder = @"F:\git";
    private const string defaultWslGitFolder = "/git";

    public string IconPath { get; set; }

    public PluginInitContext Context { get; set; }

    private Exception storageException = null;
    public SettingsModel settings;
    public SharedPluginStorage storage;

    public virtual string Name => "CorePlugin";
    public virtual string Description => "Common code for devbox plugins";

    public List<Result> Query(Query query)
    {
      if (storageException != null)
      {
        List<Result> list = new List<Result>();
        list.Add(new Result
        {
          Title = "Error loading storage",
          SubTitle = storageException.Message,
          IcoPath = IconPath
        });
        return list;
      }
      try
      {
        return OnQuery(query);
      }
      catch (Exception e)
      {
        List<Result> list = new List<Result>();
        list.Add(new Result
        {
          Title = "Devbox Plugin",
          SubTitle = "Error during query: " + e.Message,
          IcoPath = IconPath
        });
        return list;
      }
    }

    public abstract List<Result> OnQuery(Query query);

    public void Init(PluginInitContext context)
    {
      Context = context;
      Context.API.ThemeChanged += OnThemeChanged;
      UpdateIconPath(Context.API.GetCurrentTheme());

      LoadSettings();
    }
    public void ReloadData()
    {
      LoadSettings();
    }

    private void LoadSettings()
    {
      try
      {
        storage = new SharedPluginStorage(Context.API);
        settings = storage.Load();
        var shouldSave = false;
        if (string.IsNullOrEmpty(settings.GitFolder))
        {
          settings.GitFolder = defaultGitFolder;
          shouldSave = true;
        }
        if (string.IsNullOrEmpty(settings.WslGitFolder))
        {
          settings.WslGitFolder = defaultWslGitFolder;
          shouldSave = true;
        }
        if (shouldSave)
        {
          storage.Save();
        }
      }
      catch (Exception e)
      {
        storageException = e;
      }
    }

    private void UpdateIconPath(Theme theme)
    {
      if (theme == Theme.Light || theme == Theme.HighContrastWhite)
      {
        IconPath = "images/icon.light.png";
      }
      else
      {
        IconPath = "images/icon.dark.png";
      }
    }

    private void OnThemeChanged(Theme currentTheme, Theme newTheme)
    {
      UpdateIconPath(newTheme);
    }
  }
}
