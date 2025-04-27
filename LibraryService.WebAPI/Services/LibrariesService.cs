using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryService.Caching;
using LibraryService.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Services
{
    public class LibrariesService : ILibrariesService
    {
        private readonly LibraryContext _libraryContext;
        private readonly ILibraryCacheService _libraryCacheService; 
        public LibrariesService(LibraryContext libraryContext, ILibraryCacheService libraryCacheService)
        {
            _libraryContext = libraryContext;
            _libraryCacheService = libraryCacheService;
        }

        public async Task<IEnumerable<Library>> Get(int[] ids)
        {
            if(ids == null || ids.Length == 0)
            {
                return await _libraryContext.Libraries.ToListAsync();
            }
            var ret = new List<Library>();
            var missingCacheIds = new List<int>();
            foreach (var id in ids)
            {
                var cacheKey = $"library-{id}";
                var cachedLibrary = await _libraryCacheService.GetAsync<Library>(cacheKey);
                if (cachedLibrary == null)
                {
                    missingCacheIds.Add(id);
                    continue;
                }
                ret.Add(cachedLibrary);
            }
            if (missingCacheIds.Any())
            {
                var libraries = await _libraryContext.Libraries.Where(x => missingCacheIds.Contains(x.Id)).ToListAsync();
                foreach (var library in libraries)
                {
                    var cacheKey = $"library-{library.Id}";
                    await _libraryCacheService.SetAsync(cacheKey, library);
                    ret.Add(library);
                }
            }
            return ret;
        }

        public async Task<Library> Add(Library library)
        {
            await _libraryContext.Libraries.AddAsync(library);
            await _libraryContext.SaveChangesAsync();
            return library;
        }
        public async Task<Library> Update(Library library)
        {
            var projectForChanges = await _libraryContext.Libraries.SingleAsync(x => x.Id == library.Id);
            projectForChanges.Name = library.Name;
            projectForChanges.Location = library.Location;

            _libraryContext.Libraries.Update(projectForChanges);
            await _libraryContext.SaveChangesAsync();
            return library;
        }

        public async Task<bool> Delete(Library library)
        {
            var projectForDelete = await _libraryContext.Libraries.SingleAsync(x=>x.Id == library.Id);
            _libraryContext.Libraries.Remove(projectForDelete);
            await _libraryContext.SaveChangesAsync();   
            return true;
        }
    }

    public interface ILibrariesService
    {
        Task<IEnumerable<Library>> Get(int[] ids);

        Task<Library> Add(Library library);

        Task<Library> Update(Library library);

        Task<bool> Delete(Library library);
    }
}
