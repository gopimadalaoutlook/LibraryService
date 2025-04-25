using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Services;

namespace LibraryService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/libraries/{libraryId}/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILibrariesService _librariesService;
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService, ILibrariesService librariesService)
        {
            _librariesService = librariesService;
            _booksService = booksService;
        }


        [HttpGet]
        public async Task<IActionResult> Get(int libraryId)
        {
            var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
            if (library == null)
                return NotFound();
            var books = await _booksService.Get(libraryId);
            return Ok(books);
        }


        [HttpPost]
        public async Task<IActionResult> Post(int libraryId, Book book)
        {
            var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
            if (library == null)
                return NotFound();
            book.LibraryId = libraryId;
            var addedBook = await _booksService.Add(book);
            return CreatedAtAction(nameof(Get), new { libraryId = libraryId, id = addedBook.Id }, addedBook);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int libraryId, Book book)
        {
            var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
            if (library == null)
                return NotFound();
            book.LibraryId = libraryId;
            var updatedBook = await _booksService.Update(book);
            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int libraryId, int id)
        {
            var book = (await _booksService.Get(libraryId)).FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();
            var deletedBook = await _booksService.Delete(book);
            return Ok();
        }
    }
}