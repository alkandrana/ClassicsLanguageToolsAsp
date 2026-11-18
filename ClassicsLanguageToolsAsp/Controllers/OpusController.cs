using ClassicsLanguageToolsAsp.Data;
using ClassicsLanguageToolsAsp.Models;
using ClassicsLanguageToolsAsp.Models.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("opera")]
    [ApiController]
    public class OpusController : ControllerBase
    {
        private readonly IRecordRepo<Opus> _opusRepo;

        public OpusController(IRecordRepo<Opus> opusRepo)
        {
            _opusRepo = opusRepo;
        }

        // GET: opera
        [HttpGet]
        public async Task<IActionResult> GetOpera()
        {
            List<Opus> opera = await _opusRepo.GetAllRecordsAsync(qu =>
                qu.Include(w => w.Author).Include(w => w.Language)
            );
            return Ok(opera);
        }

        // GET: opera/code/MET
        [Route("/opera/code/{code}")]
        [HttpGet]
        public async Task<IActionResult> GetOpusByCode(string code)
        {
            List<Opus> opera = await _opusRepo.GetRecordByCodeAsync(code, "Code");
            if (opera.Count == 0)
            {
                return NotFound();
            }
            return Ok(opera);
        }

        // GET: opera/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOpus(int id)
        {
            Opus? opus = await _opusRepo.GetRecordByIdAsync(
                id,
                qu => qu.Include(w => w.Author).Include(w => w.Language).Include(w => w.Lines)
            );
            if (opus == null)
            {
                return NotFound();
            }

            return Ok(opus);
        }

        public async Task<IActionResult> ReadOpus(int id)
        {
            Opus? opus = await _opusRepo.GetRecordByIdAsync(
                id,
                qu => qu.Include(w => w.Author).Include(w => w.Lines)
            );
            if (opus == null)
            {
                return NotFound();
            }

            List<Line> lines = opus.Lines.ToList();
            List<string> sections = lines
                .DistinctBy(li => li.Locus)
                .Select(li => li.Locus)
                .ToList();
            Dictionary<string, Dictionary<int, string>> linesByCategory = [];
            foreach (string sec in sections)
            {
                Dictionary<int, string> linesText = new Dictionary<int, string>();
                List<Line> group = lines
                    .Where(li => li.Locus == sec)
                    .OrderBy(li => li.Number)
                    .ToList();
                foreach (Line line in group)
                {
                    linesText.Add(line.Number, line.Text);
                }
                linesByCategory.Add(sec, linesText);
            }

            ProjectVM project = new ProjectVM
            {
                Id = opus.Id,
                Code = opus.Code,
                Title = opus.Title,
                AuthorName = opus.Author != null ? opus.Author.Name : "Anonymous",
                Lines = linesByCategory,
            };

            return Ok(project);
        }

        // POST: opera
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddOpus(Opus opus)
        {
            int rowsAffected = await _opusRepo.CreateRecordAsync(opus);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create opus");
            }

            return CreatedAtAction("GetOpus", new { id = opus.Id }, opus);
        }

        // PATCH: opera/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditOpus(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<Opus> opusData
        )
        {
            Opus? opusToUpdate = await _opusRepo.GetRecordByIdAsync(id);
            if (opusToUpdate == null)
            {
                return NotFound();
            }
            opusData.ApplyTo(opusToUpdate, ModelState);
            if (!TryValidateModel(opusToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await _opusRepo.UpdateRecordAsync(opusToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update opus");
            }
            return Ok(opusToUpdate);
        }

        // DELETE: opera/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOpera(int id)
        {
            Opus? opus = await _opusRepo.GetRecordByIdAsync(id);
            if (opus == null)
            {
                return NotFound();
            }
            int rowsAffected = await _opusRepo.DeleteRecordAsync(opus);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete opus");
            }
            return NoContent();
        }
    }
}
