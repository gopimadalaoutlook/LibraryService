using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryService.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Services
{
    public class BooksService : IBooksService
    {
        private readonly LibraryContext _libraryContext;

        public BooksService(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<IEnumerable<Book>> Get(int libraryId)
        {
            var books = _libraryContext.Books.AsQueryable();
            if (libraryId > 0)
                books = books.Where(x => x.LibraryId == libraryId);
            return await books.ToListAsync();
        }

        public async Task<Book> Add(Book book)
        {
            var library = await _libraryContext.Libraries.SingleOrDefaultAsync(x => x.Id == book.LibraryId);
            if (library == null)
                throw new Exception("Library not found");
            await _libraryContext.Books.AddAsync(book);
            await _libraryContext.SaveChangesAsync();
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
            return existingBook;

        }

        public async Task<bool> Delete(Book book)
        {
            var existingBook = await _libraryContext.Books.SingleOrDefaultAsync(x => x.Id == book.Id);
            if (existingBook == null)
                throw new Exception("Book not found");
            _libraryContext.Books.Remove(existingBook);
            await _libraryContext.SaveChangesAsync();
            return true;
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
