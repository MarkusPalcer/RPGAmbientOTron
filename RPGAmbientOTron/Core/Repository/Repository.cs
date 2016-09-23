using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Security.Cryptography;
using Core.Extensions;
using Core.Persistence;
using Core.Repository.Models;
using NAudio.Wave;
using Prism.Logging;

namespace Core.Repository
{
    [Export(typeof(IRepository))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Repository : IRepository
    {
        private readonly IPersistenceService persistenceService;
        private readonly ILoggerFacade logger;

        [ImportingConstructor]
        public Repository(IPersistenceService persistenceService, ILoggerFacade logger)
        {
            this.persistenceService = persistenceService;
            this.logger = logger;

            Init();
        }

        private void Init()
        {
            persistenceService.Libraries.ForEach(LoadLibrary);
        }

        private void LoadLibrary(Core.Persistence.Models.Library persistenceModel)
        {
            var hasher = SHA256.Create();

            var library = new Library
            {
                PersistenceModel = persistenceModel
            };

            foreach (var audioFile in persistenceModel.Files)
            {
                var fileModel = new AudioFile
                {
                    PersistenceModel = audioFile
                };

                if (!File.Exists(audioFile.FileName))
                {
                    fileModel.LoadStatus = LoadStatus.FileNotFound;
                }

                try
                {
                    using (var stream = File.OpenRead(audioFile.FileName))
                    {
                        fileModel.Hash = hasher.ComputeHash(stream);
                    }

                    // Ensure the file is readable as MP3 file
                    // To be replaced by codec detection later
                    using (new Mp3FileReader(audioFile.FileName)) {}
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    fileModel.LoadStatus = LoadStatus.LoadError;
                }

                library.Files.Add(fileModel);
            }
        }
    }
}