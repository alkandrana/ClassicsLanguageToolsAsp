using Microsoft.AspNetCore.Mvc;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.JsonPatch;
using ClassicsLanguageToolsAsp.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IRecordRepo<Author> _authorRepo;

        public AuthorController(IRecordRepo<Author> authRepo)
        {
            _authorRepo = authRepo;
        }

        // GET: authors
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            List<Author> authors = await _authorRepo.GetAllRecordsAsync();
            return Ok(authors);
        }

        // GET: opera/code/VERG
        [Route("/authors/code/{code}")]
        [HttpGet]
        public async Task<IActionResult> GetAuthorByCode(string code)
        {
            List<Author> authors = await _authorRepo.GetRecordByCodeAsync(code, "Code");
            if (authors.Count == 0)
            {
                return NotFound();
            }
            return Ok(authors);
        }

        // GET: authors/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            Author? author = await _authorRepo
                .GetRecordByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // POST: opera
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddAuthor(Author author)
        {
            int rowsAffected = await _authorRepo.CreateRecordAsync(author);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create author");
            }

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }
        
        // PATCH: opera/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditAuthor([FromRoute] int id, [FromBody] JsonPatchDocument<Author> authorData)
        {
            Author? authorToUpdate = await _authorRepo
                .GetRecordByIdAsync(id);
            if (authorToUpdate == null)
            {
                return NotFound();
            }
            authorData.ApplyTo(authorToUpdate, ModelState);
            if (!TryValidateModel(authorToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await  _authorRepo.UpdateRecordAsync(authorToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update author");
            }
            return Ok(authorToUpdate);
        }

        // DELETE: opera/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            Author? author = await _authorRepo.GetRecordByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            int rowsAffected = await _authorRepo.DeleteRecordAsync(author);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete author");
            }
            return NoContent();
        }
    }
}
