using System.Linq;

namespace Core.Repository.PersistenceMocels
{
    public static class Conversion
    {
        public static Library ConvertToPersistenceModel(this Models.Library model)
        {
            var result = new Library
            {
                Name = model.Name,
            };

            foreach (var file in model.Files)
            {
                var fileModel = file.ConvertToPersistenceModel();
                fileModel.Path = MakeRelativePath(model.Path, fileModel.Path);
                result.Files.Add(fileModel);
            }

            foreach (var library in model.SatteliteLibraries)
            {
                result.SatteliteLibraryPaths.Add(MakeRelativePath(model.Path, library.Path));
            }

          result.SoundBoards.AddRange(model.SoundBoards);

            return result;
        }

        public static string MakeRelativePath(string possibleParentPath, string possibleChildPath)
        {
            return possibleChildPath.StartsWith(possibleParentPath) 
                ? $".\\{possibleChildPath.Substring(possibleParentPath.Length)}" 
                : possibleChildPath;
        }

        public static AudioFile ConvertToPersistenceModel(this Models.AudioFile model)
        {
            return new AudioFile
            {
                Name = model.Name,
                Path = model.FullPath
            };
        }
    }
}