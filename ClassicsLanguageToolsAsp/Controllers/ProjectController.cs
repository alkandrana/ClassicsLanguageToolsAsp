using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using WritingSessionsAspApi.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IRecordRepo<Project> _projectRepo;
        private readonly UserManager<ClassUser> _userManager;

        public ProjectController(IRecordRepo<Project> projRepo, UserManager<ClassUser> userManager)
        {
            _projectRepo = projRepo;
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

        // GET: projects/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProject(int id)
        {
            Project? project = await _projectRepo
                .GetRecordByIdAsync(id, qu => qu
                    .Include(p => p.VocabList)
                    .ThenInclude(i => i.Vocab));
            if (project == null)
            {
                return NotFound();
            }
            // string projectUserId = await _ctx.Projects.Where(p => p.Id == id).Select(p => p.Creator.Id).SingleOrDefaultAsync();
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.Id != project.CreatorId)
            {
                return Unauthorized();
            }

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

            int rowsAffected = await  _projectRepo.UpdateRecordAsync(projectToUpdate);
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
