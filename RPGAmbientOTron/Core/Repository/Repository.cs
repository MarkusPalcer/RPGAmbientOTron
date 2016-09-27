using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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
        private readonly SHA256 sha256 = SHA256.Create();

        private readonly Dictionary<Persistence.Models.AudioFile, AudioFile> fileModelsByPersistenceModel = new Dictionary<Persistence.Models.AudioFile, AudioFile>();

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
            var library = new Library
            {
                PersistenceModel = persistenceModel
            };

            library.Files.AddRange(persistenceModel.Files.Select(GetAudioFileModel));
        }

        private AudioFile GetAudioFileModel(Persistence.Models.AudioFile persistenceModel)
        {
            AudioFile result;
            if (fileModelsByPersistenceModel.TryGetValue(persistenceModel, out result))
            {
                return result;
            }

            result = new AudioFile
            {
                PersistenceModel = persistenceModel
            };

            if (!File.Exists(persistenceModel.FileName))
            {
                result.LoadStatus = LoadStatus.FileNotFound;
            }

            try
            {
                using (var stream = File.OpenRead(persistenceModel.FileName))
                {
                    result.Hash = sha256.ComputeHash(stream);
                }

                // Ensure the file is readable as MP3 file
                // To be replaced by codec detection later
                using (new Mp3FileReader(persistenceModel.FileName)) {}
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                result.LoadStatus = LoadStatus.LoadError;
            }

            fileModelsByPersistenceModel[persistenceModel] = result;

            return result;
        }

        public AudioFile GetAudioFileModel(string fileName)
        {
            return GetAudioFileModel(persistenceService.GetAudioFileModelFor(fileName));
        }
    }
}