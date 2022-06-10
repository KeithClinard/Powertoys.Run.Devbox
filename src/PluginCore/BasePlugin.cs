using System;
using System.Collections.Generic;
using ManagedCommon;
using Powertoys.Run.Devbox.PluginCore.Models;
using Wox.Infrastructure.Storage;
using Wox.Plugin;

namespace Powertoys.Run.Devbox.PluginCore
{
  public abstract class BasePlugin : IPlugin, IReloadable
  {

    private const string defaultGitFolder = @"C:\git";
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

      try
      {
        ReloadData();
      }
      catch (Exception e)
      {
        storageException = e;
      }
    }
    public void ReloadData()
    {
      LoadSettings();
      OnReloadData();
    }

    public virtual void OnReloadData() { }

    private void LoadSettings()
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

    private void UpdateIconPath(Theme theme)
    {
      if (theme == Theme.Light || theme == Theme.HighContrastWhite)
      {
        IconPath = "icon.light.png";
      }
      else
      {
        IconPath = "icon.dark.png";
      }
    }

    private void OnThemeChanged(Theme currentTheme, Theme newTheme)
    {
      UpdateIconPath(newTheme);
    }
  }
}
