using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Repository.Models;
using Core.Repository.Sounds;
using Core.Repository.Sources;
using log4net;
using Newtonsoft.Json;
using Prism.Events;

namespace Core.Repository
{
  [Export(typeof(IRepository))]
  [Export(typeof(IInternalRepository))]
  [PartCreationPolicy(CreationPolicy.Shared)]
  [Export]
  public class Repository : IRepository, IInternalRepository, IDisposable
  {
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    private readonly IEventAggregator eventAggregator;
    private readonly HashSet<SoundBoard> soundBoardCache = new HashSet<SoundBoard>();
    private readonly Dictionary<string, ISource> knownFiles = new Dictionary<string, ISource>();
    private readonly Dictionary<string, ISource> knownSources = new Dictionary<string, ISource>();
    private readonly Dictionary<string, BehaviorSubject<Status>> statuses = new Dictionary<string, BehaviorSubject<Status>>();
    private readonly Dictionary<string, Cache> caches = new Dictionary<string, Cache>();
    private readonly List<Ambience> ambiences = new List<Ambience>();

    private readonly ILog logger = LogManager.GetLogger(typeof(Repository));
    private readonly string rootLibraryFileName = "Data.json";
    private readonly JsonSerializerSettings jsonSerializerSettings;

    [ImportingConstructor]
    public Repository(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
      jsonSerializerSettings = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Auto,
      };

      eventAggregator.OnModelAdd<SoundBoard>(model => soundBoardCache.Add(model));
      eventAggregator.OnModelAdd<Ambience>(newModel => ambiences.Add(newModel));
    }

    public async void Init()
    {
      using (await semaphore.ProtectAsync())
      {
        await LoadLibrary();

        if (!caches.ContainsKey(Environment.CurrentDirectory))
        {
          await ImportCache(
            new Cache
            {
              Folder = Environment.CurrentDirectory,
              Name = "Program folder"
            });
        }
      }


    }

    private async Task LoadLibrary()
    {
      var fullPath = Path.Combine(Environment.CurrentDirectory, rootLibraryFileName);
      if (!File.Exists(fullPath))
      {
        logger.Info($"Library {fullPath} does not exist.");
        return;
      }

      try
      {
        var model = JsonConvert.DeserializeObject<Library>(File.ReadAllText(fullPath), jsonSerializerSettings);

        ImportSoundBoards(model);

        ImportAmbiences(model);

        foreach (var cache in model.Caches)
        {
          await ImportCache(cache);
        }

        await ImportFiles(model);
      }
      catch (Exception ex)
      {
        logger.Warn($"Could not load library from {fullPath}", ex);
      }
    }

    private void ImportAmbiences(Library model)
    {
      foreach (var ambience in model.Ambiences)
      {
        ambiences.Add(ambience);

        ambience.Entries.OfType<Loop>().ForEach(x => x.Sound.Status = CreateOrSetStatus(x.Sound.Hash, Status.NotFound));

        eventAggregator.ModelAdded(ambience);
      }
    }

    private async Task ImportFiles(Library model)
    {
      // Try to read all files in parallel
      var fileTasks = model.Files.Select(
        async fileName =>
        {
          AudioFile result = null;
          try
          {
            result = await Task.Factory.StartNew(() => new AudioFile(fileName));
          }
          catch (Exception ex)
          {
            logger.Error($"Error while trying to create source for file '{fileName}'", ex);
          }

          return new
          {
            FileName = fileName,
            Source = result
          };
        })
        .ToArray();

      // Synchronize
      var files = await Task.WhenAll(fileTasks);

      // Throw away files that couldn't be loaded and only keep one file per hash
      files = files.Where(x => x.Source != null).Distinct().ToArray();

      foreach (var file in files)
      {
        // Ignore files that are not "requested" by other entities
        BehaviorSubject<Status> status;
        if (!statuses.TryGetValue(file.Source.Hash, out status))
        {
          continue;
        }

        // Register file source and set file status
        knownFiles[file.FileName] = file.Source;
        knownSources[file.Source.Hash] = file.Source;
        status.OnNext(Status.Ready);
      }
    }

    private void ImportSoundBoards(Library model)
    {
      foreach (var soundBoard in model.SoundBoards)
      {
        soundBoardCache.Add(soundBoard);

        foreach (var sound in soundBoard.Entries)
        {
          sound.Sound.Status = CreateOrSetStatus(sound.Sound.Hash, Status.NotFound);
        }

        eventAggregator.ModelAdded(soundBoard);
      }
    }

    private async Task ImportCache(Cache model)
    {
      if (!Directory.Exists(model.Folder))
      {
        return;
      }

      eventAggregator.ModelAdded(model);

      foreach (var fileName in Directory.EnumerateFiles(model.Folder, "*.mp3", SearchOption.AllDirectories))
      {
        var source = await ResolveFileSource(fileName);

        if (source == null)
        {
          continue;
        }

        model.Sounds.Add(ResolveSound(source, new FileInfo(fileName).Name));

        eventAggregator.ModelUpdated(model);
      }

      caches.Add(model.Folder, model);
    }

   
    private void SaveLibrary()
    {
      var rootLibrary = new Library();
      rootLibrary.SoundBoards.AddRange(soundBoardCache);
      rootLibrary.Files.AddRange(knownFiles.Keys);
      rootLibrary.Caches.AddRange(caches.Values);
      rootLibrary.Ambiences.AddRange(ambiences);

      var rootLibraryString = JsonConvert.SerializeObject(rootLibrary, Formatting.Indented, jsonSerializerSettings);

      var path = Path.Combine(Environment.CurrentDirectory, rootLibraryFileName);

      try
      {
        File.WriteAllText(path, rootLibraryString);
      }
      catch (Exception ex)
      {
        logger.Warn($"Error while writing library to '{path}'", ex);
      }
    }

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
        SaveLibrary();
      }

      // Release unmanaged resources here
    }

    ISource IInternalRepository.GetSource(Sound sound)
    {
      ISource result;
      return knownSources.TryGetValue(sound.Hash, out result) ? result : null;
    }

    public async Task<Sound> ImportFile(string fileName)
    {
      using (await semaphore.ProtectAsync())
      {
        var source = await ResolveFileSource(fileName);

        return source == null ? null : ResolveSound(source, new FileInfo(fileName).Name);
      }
    }

    public async Task ImportCache(string cacheFolder)
    {
      using (await semaphore.ProtectAsync())
      {
        if (caches.ContainsKey(cacheFolder))
          return;

        await ImportCache(
          new Cache
          {
            Folder = cacheFolder,
            Name = new DirectoryInfo(cacheFolder).Name

          });
      }
    }

    private Sound ResolveSound(ISource source, string defaultName)
    {
      var result = new Sound
      {
        Name = defaultName,
        Hash = source.Hash
      };

      result.Status = statuses[result.Hash];

      return result;
    }

    private async Task<ISource> ResolveFileSource(string fileName)
    {
      ISource result;
      if (knownFiles.TryGetValue(fileName, out result))
      {
        return result;
      }

      try
      {
        result = await Task.Factory.StartNew(() => new AudioFile(fileName));
        knownFiles[fileName] = result;
        knownSources[result.Hash] = result;
      }
      catch (Exception ex)
      {
        logger.Error($"Error while trying to create source for file '{fileName}'", ex);
      }

      if (result == null)
      {
        return null;
      }

      CreateOrSetStatus(result.Hash, Status.Ready);

      return result;
    }

    private IObservable<Status> CreateOrSetStatus(string hash, Status newValue)
    {
      BehaviorSubject<Status> result;
      if (!statuses.TryGetValue(hash, out result))
      {
        result = new BehaviorSubject<Status>(newValue);
        statuses[hash] = result;
      }
      else
      {
        result.OnNext(newValue);
      }

      return result;
    }
  }
}
