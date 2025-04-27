using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Services;
using System;
using LibraryService.WebAPI.DTO;

namespace LibraryService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibrariesController : ControllerBase
    {
        private readonly ILibrariesService _librariesService;

        public LibrariesController(ILibrariesService librariesService)
        {
            _librariesService = librariesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiGetListResponse<Library>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var libraries = await _librariesService.Get(null);
                return Ok(libraries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{libraryId}")]
        [ProducesResponseType(typeof(Library), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int libraryId)
        {
            try
            {
                var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (library == null)
                    return NotFound();
                return Ok(library);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Library), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(Library library)
        {
            try
            {
                await _librariesService.Add(library);
                return CreatedAtAction(nameof(Get), new { libraryId = library.Id }, library);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{libraryId}")]
        [ProducesResponseType(typeof(Library), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int libraryId, Library library)
        {
            try
            {
                var existingLibrary = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (existingLibrary == null)
                    return NotFound();
                await _librariesService.Update(library);
                return Ok(library);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{libraryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Delete(int libraryId)
        {
            try
            {
                var existingLibrary = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (existingLibrary == null)
                    return NotFound();
                await _librariesService.Delete(existingLibrary);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
