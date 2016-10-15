using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Events;
using Core.Extensions;
using Core.Logging;
using Core.Persistence;
using Core.Repository.Models;
using Core.Repository.PersistenceMocels;
using NAudio.Wave;
using Newtonsoft.Json;
using Prism.Events;
using AudioFile = Core.Repository.Models.AudioFile;
using Library = Core.Repository.Models.Library;

namespace Core.Repository
{
  [Export(typeof(IRepository))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class Repository : IRepository, IDisposable
  {
    private static readonly string[] RootLibraryPaths = {Environment.CurrentDirectory};

    private readonly Dictionary<string, AudioFile> audioFileCache = new Dictionary<string, AudioFile>();
    private readonly IEventAggregator eventAggregator;
    private readonly Dictionary<string, Library> libraryCache = new Dictionary<string, Library>();

    private readonly ILoggingService logger;
    private readonly string rootLibraryFileName = $"root.{Constants.LibraryExtension}";


    [ImportingConstructor]
    public Repository(ILoggingService logger, IEventAggregator eventAggregator)
    {
      this.logger = logger;
      this.eventAggregator = eventAggregator;

      Init();
    }

    public Library GetLibraryModel(string path)
    {
      return LoadLibrary(path, true);
    }

    public AudioFile GetAudioFileModel(string fileName)
    {
      return LoadAudioFile(fileName);
    }

    public IEnumerable<Library> Libraries => libraryCache.Values;

    public void Save(Library model)
    {
      var exists = libraryCache.ContainsKey(model.Path);
      libraryCache[model.Path] = model;

      File.WriteAllText(model.Path, JsonConvert.SerializeObject(model.ConvertToPersistenceModel()));

      if (exists)
      {
        eventAggregator.GetEvent<UpdateModelEvent<Library>>().Publish(model);
      }
      else
      {
        eventAggregator.GetEvent<AddModelEvent<Library>>().Publish(model);
      }
    }

    void IRepository.LoadLibrary(string path)
    {
      if (libraryCache.ContainsKey(path))
      {
        return;
      }

      LoadLibrary(path, true);
    }

    private void Init()
    {
      RootLibraryPaths.Select(p => Path.Combine(p, rootLibraryFileName)).ForEach(x => LoadLibrary(x, false));
    }

    private Library LoadLibrary(string fullPath, bool addToLibraryList)
    {
      Library result;
      if (libraryCache.TryGetValue(fullPath, out result))
      {
        return result;
      }

      if (!File.Exists(fullPath))
      {
        logger.Info<Repository>($"Library {fullPath} does not exist.");
        return null;
      }

      try
      {
        var persistenceModel = JsonConvert.DeserializeObject<PersistenceMocels.Library>(File.ReadAllText(fullPath));
        result = new Library
        {
          Path = fullPath,
          Name = persistenceModel.Name
        };

        persistenceModel.SatteliteLibraryPaths.Select(x => ResolveLink(x, fullPath))
                        .Select(x => LoadLibrary(x, true))
                        .Where(x => x != null)
                        .ForEach(result.SatteliteLibraries.Add);

        persistenceModel.Files.Select(x => LoadAudioFile(ResolveLink(x.Path, fullPath), x))
                        .Where(x => x != null)
                        .ForEach(result.Files.Add);
      }
      catch (Exception ex)
      {
        logger.Warn<Repository>($"Could not load library from {fullPath}", ex);
        return null;
      }

      if (addToLibraryList)
      {
        libraryCache[fullPath] = result;
        eventAggregator.GetEvent<AddModelEvent<Library>>().Publish(result);
      }

      return result;
    }

    private AudioFile LoadAudioFile(string fullPath, PersistenceMocels.AudioFile persistenceModel = null)
    {
      AudioFile result;
      if (audioFileCache.TryGetValue(fullPath, out result))
      {
        return result;
      }

      result = new AudioFile
      {
        FullPath = fullPath,
        Name = persistenceModel?.Name ?? new FileInfo(fullPath).Name,
        LoadStatus = LoadStatus.Unknown
      };

      audioFileCache[fullPath] = result;

      Task.Factory.StartNew(
        () =>
        {

          if (!File.Exists(fullPath))
          {
            result.LoadStatus = LoadStatus.FileNotFound;
          }

          try
          {
            // Ensure the file is readable as MP3 file
            // To be replaced by codec detection later
            using (new Mp3FileReader(fullPath)) {}

            result.LoadStatus = LoadStatus.FileOk;
          }
          catch (Exception ex)
          {
            logger.Warn<Repository>($"Could not load audio file from {fullPath}", ex);
            result.LoadStatus = LoadStatus.LoadError;
          }

          eventAggregator.GetEvent<UpdateModelEvent<AudioFile>>().Publish(result);
        });

      return result;
    }

    private string ResolveLink(string path, string parentPath)
    {
      return path.StartsWith(".") ? Path.Combine(parentPath, path) : path;
    }

    private void SaveRootLibraries()
    {
      var rootLibrary = new PersistenceMocels.Library();
      rootLibrary.SatteliteLibraryPaths.AddRange(libraryCache.Values.Select(x => x.Path));
      var rootLibraryString = JsonConvert.SerializeObject(rootLibrary);

      foreach (var path in RootLibraryPaths.Select(p => Path.Combine(p, rootLibraryFileName)))
      {
        try
        {
          File.WriteAllText(path, rootLibraryString);
        }
        catch (Exception ex)
        {
          logger.Warn<Repository>($"Error while writing root library to '{path}'", ex);
        }
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
