using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Core.Extensions;
using Core.Persistence;
using Core.Repository.Models;
using Core.Repository.PersistenceMocels;
using NAudio.Wave;
using Newtonsoft.Json;
using Prism.Logging;
using AudioFile = Core.Repository.Models.AudioFile;
using Library = Core.Repository.Models.Library;

namespace Core.Repository
{
    [Export(typeof(IRepository))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Repository : IRepository
    {
        private readonly string rootLibraryFileName = $"root.{Constants.LibraryExtension}";

        private static readonly string[] RootLibraryPaths = { Environment.CurrentDirectory };

        private readonly ILoggerFacade logger;
        private readonly SHA256 sha256 = SHA256.Create();

        private readonly Dictionary<string, AudioFile> audioFileCache = new Dictionary<string, AudioFile>();
        private readonly Dictionary<string, Library> libraryCache = new Dictionary<string, Library>();


        [ImportingConstructor]
        public Repository(ILoggerFacade logger)
        {
            this.logger = logger;

            Init();
        }

        private void Init()
        {
            RootLibraryPaths
                .Select(p => Path.Combine(p, rootLibraryFileName))
                .ForEach(x => LoadLibrary(x));
        }

        private Library LoadLibrary(string fullPath)
        {
            Library result;
            if (libraryCache.TryGetValue(fullPath, out result))
            {
                return result;
            }

            if (!File.Exists(fullPath))
            {
                logger.Log($"Library {fullPath} does not exist.", Category.Info, Priority.Low);
                return null;
            }

            try
            {
                var persistenceModel = JsonConvert.DeserializeObject<PersistenceMocels.Library>(File.ReadAllText(fullPath));
                result = new Library
                {
                    Path = fullPath,
                };

                persistenceModel.SatteliteLibraryPaths
                    .Select(x => ResolveLink(x, fullPath))
                    .Select(LoadLibrary)
                    .Where(x => x != null)
                    .ForEach(result.SatteliteLibraries.Add);

                persistenceModel.Files
                    .Select(x => LoadAudioFile(ResolveLink(x.Path, fullPath), x))
                    .Where(x => x != null)
                    .ForEach(result.Files.Add);


                
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return null;
            }

            libraryCache[fullPath] = result;
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
                Name = persistenceModel?.Name ?? new FileInfo(fullPath).Name
            };

            if (!File.Exists(fullPath))
            {
                result.LoadStatus = LoadStatus.FileNotFound;
            }

            try
            {
                using (var stream = File.OpenRead(fullPath))
                {
                    result.Hash = sha256.ComputeHash(stream);
                }

                // Ensure the file is readable as MP3 file
                // To be replaced by codec detection later
                using (new Mp3FileReader(fullPath)) { }

                result.LoadStatus = LoadStatus.FileOk;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                result.LoadStatus = LoadStatus.LoadError;
            }

            audioFileCache[fullPath] = result;
            return result;
        }

        private string ResolveLink(string path, string parentPath)
        {
            return path.StartsWith(".") 
                ? Path.Combine(parentPath, path) 
                : path;
        }

        public AudioFile GetAudioFileModel(string fileName)
        {
            return LoadAudioFile(fileName);
        }

        public IEnumerable<Library> Libraries => libraryCache.Values;

        public void Save(Library model)
        {
            libraryCache[model.Path] = model;

            File.WriteAllText(model.Path, JsonConvert.SerializeObject(model.ConvertToPersistenceModel()));
        }
    }
}