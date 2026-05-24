using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassicsLanguageToolsAsp.Data;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("languages")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public LanguageController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET: language
        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            List<Language> languages = await _ctx.Languages.ToListAsync();
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
            List<Language> languages = await _ctx.Languages.Where(l => l.Name == name).ToListAsync();
            return Ok(languages);
        }

        // GET: language/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLanguage(int id)
        {
            Language? language = await _ctx.Languages.FindAsync(id);

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
            _ctx.Languages.Add(language);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction("GetLanguage", new { id = language.Id }, language);
        }
        
        // PUT: language/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage([FromRoute] int id, [FromBody] Language language)
        {
            if (language == null)
            {
                return BadRequest();
            }
            
            Language? langToUpdate = await _ctx.Languages.FindAsync(id);
            if (langToUpdate == null)
            {
                return NotFound();
            }

            langToUpdate.Name = language.Name;
            await _ctx.SaveChangesAsync();

            return Ok(langToUpdate);
        }

        // DELETE: language/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _ctx.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            _ctx.Languages.Remove(language);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
