using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WritingSessionsAspApi.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("languages")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly IRecordRepo<Language> _langRepo;

        public LanguageController(IRecordRepo<Language> langRepo)
        {
            _langRepo = langRepo;
        }

        // GET: language
        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            List<Language> languages = await _langRepo.GetAllRecordsAsync();
            return Ok(languages);
        }

        [HttpGet]
        [Route("/languages/name/{name}")]
        public async Task<IActionResult> GetLanguageByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }
            List<Language> languages = await _langRepo
                .GetSelectRecordsAsync(l => l.Name == name);
            return Ok(languages);
        }

        // GET: language/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLanguage(int id)
        {
            Language? language = await _langRepo.GetRecordByIdAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return Ok(language);
        }

        // POST: language
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostLanguage([FromBody] Language language)
        {
            List<Language> duplicates = await _langRepo
                .GetSelectRecordsAsync(l => l.Name == language.Name);
            if (duplicates.Count > 0)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Duplicate name",
                    Detail = "That language already exists."
                });
            }
            int rowsAffected = await _langRepo.CreateRecordAsync(language);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create language");
            }

            return CreatedAtAction("GetLanguage", new { id = language.Id }, language);
        }

        // PUT: language/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage(
            [FromRoute] int id,
            [FromBody] Language language
        )
        {
            int? rowsAffected = await _langRepo.PutRecordAsync(language, id);
            if (rowsAffected == null)
            {
                return NotFound();
            }
            if (rowsAffected == 0)
            {
                return Problem("Failed to update language");
            }
            return NoContent();
        }

        // DELETE: language/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _langRepo.GetRecordByIdAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            int rowsAffected = await _langRepo.DeleteRecordAsync(language);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete language");
            }

            return NoContent();
        }
    }
}
