using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Core.Persistence.Models;

namespace Core.Persistence
{
    public interface IPersistenceService
    {
        IEnumerable<Library> Libraries { get; }
        Library LoadLibrary(string path);
    }
}