using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassicsLanguageToolsAsp.Models;
using ClassicsLanguageToolsAsp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using ClassicsLanguageToolsAsp.Data;
using System.Security.Permissions;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("projects")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IRecordRepo<Project> _projectRepo;
        private readonly IRecordRepo<Opus> _opusRepo;
        private readonly IRecordRepo<VocabInstance> _instanceRepo;
        private readonly UserManager<ClassUser> _userManager;

        public ProjectController(IRecordRepo<Project> projRepo, IRecordRepo<Opus> opusRepo, IRecordRepo<VocabInstance> instRepo, UserManager<ClassUser> userManager)
        {
            _projectRepo = projRepo;
            _opusRepo = opusRepo;
            _instanceRepo = instRepo;
            _userManager = userManager;
        }

        // GET: projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            List<Project> projects = await _projectRepo
                .GetSelectRecordsAsync(p => p.CreatorId == currentUser.Id);
            return Ok(projects);
        }

        //GET: projects/work/5
        [HttpGet("work/{id:int}")]
        public async Task<IActionResult> GetProjectByWork(int id)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            List<Project> projects = await _projectRepo
            .GetSelectRecordsAsync(p => p.OpusId == id &&
            p.CreatorId == currentUser.Id);
            if (projects.Count == 0)
            {
                return NotFound();
            }
            return Ok(projects);
        }

        // GET: projects/5
        [HttpGet("{opusId:int}")]
        public async Task<IActionResult> GetProject(int opusId)
        {
            Opus? opus = await _opusRepo
                .GetRecordByIdAsync(opusId, qu => qu.Include(w => w.Author).Include(w => w.Lines));
            Console.WriteLine($"Opus Located: {opus != null}");
            if (opus == null)
            {
                return NotFound();
            }
            List<VocabInstance> vocabReferences = await _instanceRepo.GetSelectRecordsAsync(vr => vr.Line.OpusId == opusId, qu => qu.Include(vr => vr.Vocab));

            List<Line> lines = opus.Lines.ToList();
            List<string> sections = lines.DistinctBy(li => li.Locus).Select(li => li.Locus).ToList();
            Dictionary<string, Dictionary<int, string>> linesByCategory = [];
            foreach (string sec in sections)
            {
                Dictionary<int, string> linesText = new Dictionary<int, string>();
                List<Line> group = lines.Where(li => li.Locus == sec).OrderBy(li => li.Number).ToList();
                foreach (Line line in group)
                {
                    linesText.Add(line.Number, line.Text);
                }
                linesByCategory.Add(sec, linesText);
            }

            ProjectVM project = new ProjectVM
            {
                Code = opus.Code,
                Title = opus.Title,
                AuthorName = opus.Author != null ? opus.Author.Name : "Anonymous",
                Lines = linesByCategory,
                Vocab = vocabReferences
            };

            return Ok(project);
        }

        // POST: projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddProject(Project project)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            project.Creator = currentUser;
            project.StartDate = DateTime.Now;
            int rowsAffected = await _projectRepo.CreateRecordAsync(project);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create project");
            }

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // PATCH: projects/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditProject([FromRoute] int id, [FromBody] JsonPatchDocument<Project> projectData)
        {
            Project? projectToUpdate = await _projectRepo
                .GetRecordByIdAsync(id, qu => qu
                    .Include(p => p.Creator));
            if (projectToUpdate == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            if (currentUser.Id != projectToUpdate.CreatorId)
            {
                return Forbid();
            }
            projectData.ApplyTo(projectToUpdate, ModelState);
            if (!TryValidateModel(projectToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await _projectRepo.UpdateRecordAsync(projectToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update project");
            }
            return Ok(projectToUpdate);
        }

        // DELETE: projects/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _projectRepo.GetRecordByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (currentUser.Id != project.CreatorId)
            {
                return Forbid();
            }
            int rowsAffected = await _projectRepo.DeleteRecordAsync(project);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete project");
            }
            return NoContent();
        }
    }
}
