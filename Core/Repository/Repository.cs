using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Core.Events;
using Core.Repository.Models;
using Core.Repository.Models.Sources;
using log4net;
using Newtonsoft.Json;
using Prism.Events;
using AudioFile = Core.Repository.Models.Sources.AudioFile;
using Library = Core.Repository.Models.Library;

namespace Core.Repository
{
  [Export(typeof(IRepository))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class Repository : IRepository, IDisposable
  {
    private readonly IEventAggregator eventAggregator;
    private readonly Dictionary<Guid, SoundBoard> soundBoardCache = new Dictionary<Guid, SoundBoard>();
    private readonly Dictionary<string, AudioFile> knownFiles = new Dictionary<string, AudioFile>();

    private readonly ILog logger = LogManager.GetLogger(typeof(Repository));
    private readonly string rootLibraryFileName = "Data.json";


    [ImportingConstructor]
    public Repository(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;

      Init();
    }

    public void Save(SoundBoard model)
    {
      var exists = soundBoardCache.ContainsKey(model.Id);
      soundBoardCache[model.Id] = model;

      if (exists)
      {
        eventAggregator.GetEvent<UpdateModelEvent<SoundBoard>>().Publish(model);
      }
      else
      {
        eventAggregator.GetEvent<AddModelEvent<SoundBoard>>().Publish(model);
      }
    }

    public AudioFile GetSource(string fileName)
    {
      // Currently there is only one source that can be referenced from a file
      if (knownFiles.ContainsKey(fileName))
      {
        return knownFiles[fileName];
      }

      var result = new AudioFile()
      {
        FullPath = fileName,
        Name = new FileInfo(fileName).Name,
      };

      return result;
    }

    private void Init()
    {
      LoadLibrary(Path.Combine(Environment.CurrentDirectory, rootLibraryFileName));
    }

    private void LoadLibrary(string fullPath)
    {
      if (!File.Exists(fullPath))
      {
        logger.Info($"Library {fullPath} does not exist.");
        return;
      }

      try
      {
        var model = JsonConvert.DeserializeObject<Library>(File.ReadAllText(fullPath));

        foreach (var soundBoard in model.SoundBoards)
        {
          soundBoardCache[soundBoard.Id] = soundBoard;
        }
      }
      catch (Exception ex)
      {
        logger.Warn($"Could not load library from {fullPath}", ex);
      }
    }

    public SoundBoard LoadSoundBoard(Guid id)
    {
      SoundBoard result;
      return soundBoardCache.TryGetValue(id, out result) ? result : null;
    }

    public IEnumerable<SoundBoard> GetSoundBoards()
    {
      return soundBoardCache.Values.ToArray();
    }

    private void SaveRootLibraries()
    {
      var rootLibrary = new Library();
      rootLibrary.SoundBoards.AddRange(soundBoardCache.Values);
      var rootLibraryString = JsonConvert.SerializeObject(rootLibrary, Formatting.Indented, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Auto,
      });

      var path = Path.Combine(Environment.CurrentDirectory, rootLibraryFileName);

      try
      {
        File.WriteAllText(path, rootLibraryString);
      }
      catch (Exception ex)
      {
        logger.Warn($"Error while writing root library to '{path}'", ex);
      }
    }

    #region Implementation of IDisposable

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        SaveRootLibraries();
      }

      // Release unmanaged resources here
    }

    #endregion
  }
}
