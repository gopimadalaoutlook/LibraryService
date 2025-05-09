﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Services;
using LibraryService.WebAPI.DTO;
using Microsoft.AspNetCore.Authorization;

namespace LibraryService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/libraries/{libraryId}/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ILibrariesService _librariesService;
        private readonly IBooksService _booksService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBooksService booksService, ILibrariesService librariesService, ILogger<BooksController> logger)
        {
            _librariesService = librariesService;
            _booksService = booksService;                
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiGetListResponse<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int libraryId)
        {
            try
            {
                var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (library == null)
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                var books = await _booksService.Get(libraryId);
                return Ok(new ApiGetListResponse<Book> { Count = books.Count(), Items = books });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting books for library {libraryId}");
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(int libraryId, Book book)
        {
            try
            {
                var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (library == null)
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                book.LibraryId = libraryId;
                var addedBook = await _booksService.Add(book);
                return CreatedAtAction(nameof(Get), new { libraryId = libraryId, id = addedBook.Id }, addedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding book to library {libraryId}");
                return StatusCode(500, ex.Message);
            }

        }


        [HttpPut]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int libraryId, Book book)
        {
            try
            {
                var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (library == null)
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                book.LibraryId = libraryId;
                var updatedBook = await _booksService.Update(book);
                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book in library {libraryId}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int libraryId, int id)
        {
            try
            {
                var book = (await _booksService.Get(libraryId)).FirstOrDefault(x => x.Id == id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with id {id} not found in library {libraryId}");
                    return NotFound();
                }
                var deletedBook = await _booksService.Delete(book);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with id {id} from library {libraryId}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}