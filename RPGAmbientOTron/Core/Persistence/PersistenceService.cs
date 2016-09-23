using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Core.Extensions;
using Core.Persistence.Models;
using Newtonsoft.Json;
using Prism.Logging;

namespace Core.Persistence
{
    [Export(typeof(IPersistenceService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PersistenceService : IPersistenceService
    {
        private readonly string rootLibraryFileName = $"root{Constants.LibraryExtension}";

        private static readonly string[] RootLibraryPaths = { Environment.CurrentDirectory };
        
        private readonly ILoggerFacade logger;

        private readonly List<Library> libraries = new List<Library>();


        [ImportingConstructor]
        public PersistenceService(ILoggerFacade logger)
        {
            this.logger = logger;
            Init();
        }

        public IEnumerable<Library> Libraries => libraries;

        private void Init()
        {
            var knownLibraryFiles = new HashSet<string>();
            var libraryQueue = new Queue<string>(RootLibraryPaths.Select(p => Path.Combine(p, rootLibraryFileName)));

            while (libraryQueue.Any())
            {
                var path = libraryQueue.Dequeue();
                if (knownLibraryFiles.Contains(path)) continue;

                knownLibraryFiles.Add(path);
                var library = LoadLibrary(path);

                if (library == null) continue;

                libraries.Add(library);
            }
        }



        public Library LoadLibrary(string path)
        {
            if (!File.Exists(path))
            {
                logger.Log($"Library {path} does not exist.", Category.Info, Priority.Low);
                return null;
            }

            try
            {
                var result = JsonConvert.DeserializeObject<Library>(File.ReadAllText(path));

                result.FileName = path;
                return result;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return null;
            }
        }
    }
}
