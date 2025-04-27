using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryService.Caching;
using LibraryService.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Services
{
    public class BooksService : IBooksService
    {
        private readonly LibraryContext _libraryContext;
        private readonly ILibraryCacheService _libraryCacheService;

        public BooksService(LibraryContext libraryContext, ILibraryCacheService libraryCacheService)
        {
            _libraryContext = libraryContext;
            _libraryCacheService = libraryCacheService;
        }

        public async Task<IEnumerable<Book>> Get(int libraryId)
        {
            var bookcacheKey = $"booklibrary-{libraryId}";
            var cachedBooks = await _libraryCacheService.GetAsync<IEnumerable<Book>>(bookcacheKey);
            if (cachedBooks != null)
                return cachedBooks;

            var books = _libraryContext.Books.AsQueryable();
            if (libraryId > 0)
                books = books.Where(x => x.LibraryId == libraryId);

            await _libraryCacheService.SetAsync(bookcacheKey, books.ToList());

            return await books.ToListAsync();
        }

        public async Task<Book> Add(Book book)
        {
            var bookcacheKey = $"booklibrary-{book.LibraryId}";
            var libraryCacheKey = $"library-{book.LibraryId}";
            var library = await _libraryCacheService.GetAsync<Library>(libraryCacheKey);
            if (library == null)
            {
                library = await _libraryContext.Libraries.SingleOrDefaultAsync(x => x.Id == book.LibraryId);
                if (library == null)
                    throw new Exception("Library not found");
                await _libraryCacheService.SetAsync(libraryCacheKey, library);
            }
            await _libraryContext.Books.AddAsync(book);
            await _libraryContext.SaveChangesAsync();

            var books = await _libraryContext.Books.Where(x => x.LibraryId == book.LibraryId).ToListAsync();
            await ResetBookCache(book.LibraryId, books);

            return book;
        }

        public async Task<Book> Update(Book book)
        {
            var existingBook = await _libraryContext.Books.SingleOrDefaultAsync(x => x.Id == book.Id);
            if (existingBook == null)
                throw new Exception("Book not found");
            existingBook.Name = book.Name;
            existingBook.Category = book.Category;
            existingBook.LibraryId = book.LibraryId;
            _libraryContext.Books.Update(existingBook);
            await _libraryContext.SaveChangesAsync();

            var books = await _libraryContext.Books.Where(x => x.LibraryId == book.LibraryId).ToListAsync();
            await ResetBookCache(book.LibraryId, books);

            return existingBook;

        }

        public async Task<bool> Delete(Book book)
        {
            var existingBook = await _libraryContext.Books.SingleOrDefaultAsync(x => x.Id == book.Id);
            if (existingBook == null)
                throw new Exception("Book not found");
            _libraryContext.Books.Remove(existingBook);
            await _libraryContext.SaveChangesAsync();
                        var bookcacheKey = $"booklibrary-{book.LibraryId}";

            var books = await _libraryContext.Books.Where(x => x.LibraryId == book.LibraryId).ToListAsync();
            await ResetBookCache(book.LibraryId, books);

            return true;
        }

        public async Task ResetBookCache(int libraryId, List<Book> books)
        {
            var bookcacheKey = $"booklibrary-{libraryId}";
            await _libraryCacheService.RemoveAsync(bookcacheKey);
            await _libraryCacheService.SetAsync(bookcacheKey, books);
        }
    }

    public interface IBooksService
    {
        Task<IEnumerable<Book>> Get(int libraryId);

        Task<Book> Add(Book book);

        Task<Book> Update(Book book);

        Task<bool> Delete(Book book);
    }
}
