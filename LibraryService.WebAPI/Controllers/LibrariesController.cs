using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Services;
using System;
using LibraryService.WebAPI.DTO;
using Microsoft.AspNetCore.Authorization;

namespace LibraryService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibrariesController : ControllerBase
    {
        private readonly ILibrariesService _librariesService;
        private readonly ILogger<LibrariesController> _logger;

        public LibrariesController(ILibrariesService librariesService, ILogger<LibrariesController> logger)
        {
            _librariesService = librariesService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiGetListResponse<Library>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var libraries = await _librariesService.Get(null);
                return Ok(new ApiGetListResponse<Library> { Count = libraries.Count(), Items = libraries });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting libraries");
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
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                return Ok(library);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting library {libraryId}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Library), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Library library)
        {
            try
            {
                await _librariesService.Add(library);
                return CreatedAtAction(nameof(Get), new { libraryId = library.Id }, library);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding library");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{libraryId}")]
        [ProducesResponseType(typeof(Library), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int libraryId, Library library)
        {
            try
            {
                var existingLibrary = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (existingLibrary == null)
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                await _librariesService.Update(library);
                return Ok(library);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating library {libraryId}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{libraryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int libraryId)
        {
            try
            {
                var existingLibrary = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
                if (existingLibrary == null)
                {
                    _logger.LogWarning($"Library with id {libraryId} not found");
                    return NotFound();
                }
                await _librariesService.Delete(existingLibrary);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting library {libraryId}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
