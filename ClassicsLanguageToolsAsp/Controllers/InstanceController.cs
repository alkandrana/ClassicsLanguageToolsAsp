using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using WritingSessionsAspApi.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("instances")]
    [ApiController]
    [Authorize]
    public class InstanceController : ControllerBase
    {
        private readonly IRecordRepo<VocabInstance> _instanceRepo;
        private readonly IRecordRepo<Vocab> _vocabRepo;
        private readonly UserManager<ClassUser> _userManager;

        public InstanceController(IRecordRepo<VocabInstance> instRep, IRecordRepo<Vocab> vocabRepo, UserManager<ClassUser> userMng)
        {
            _vocabRepo = vocabRepo;
            _instanceRepo = instRep;
            _userManager = userMng;
        }

        // GET: instances
        [HttpGet]
        public async Task<IActionResult> GetInstances()
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            List<VocabInstance> instances = await _instanceRepo
                .GetSelectRecordsAsync(v => v.Vocab.CreatorId == currentUser.Id);
            return Ok(instances);
        }
        
        [HttpGet]
        [Route("/instances/project/{id:int}")]
        public async Task<IActionResult> GetInstancesByProject(int id)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            List<VocabInstance> vocab = await _instanceRepo
                .GetSelectRecordsAsync(
                i => i.Vocab.CreatorId == currentUser.Id && i.ProjectId == id, 
                qu => qu.Include(i => i.Vocab));
            return Ok(vocab);
        }

        // GET: instances/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstance(int id)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            VocabInstance? instance =
                await _instanceRepo
                    .GetRecordByIdAsync(
                        id, 
                        qu => qu
                            .Include(i => i.Vocab)
                        );
            if (instance == null)
            {
                return NotFound();
            }

            if (instance.Vocab?.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }

            return Ok(instance);
        }

        // POST: instances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddInstance(VocabInstance instance)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            Vocab? parent = await _vocabRepo
                .GetRecordByIdAsync(
                    instance.VocabId, 
                    qu => qu.Include(v => v.Creator)
                );
            if (parent == null)
            {
                return NotFound();
            }

            if (parent.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }

            int rowsAffected = await _instanceRepo.CreateRecordAsync(instance);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create vocab instance.");
            }
            return CreatedAtAction("GetInstance", new { id = instance.Id }, instance);
        }
        
        // PATCH: instances/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditInstance([FromRoute] int id, [FromBody] JsonPatchDocument<VocabInstance> instanceData)
        {
            VocabInstance? instanceToUpdate = await _instanceRepo
                .GetRecordByIdAsync(id, qu => qu
                        .Include(i => i.Vocab)
                        .ThenInclude(v => v.Creator));
            if (instanceToUpdate == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || instanceToUpdate.Vocab?.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }
            instanceData.ApplyTo(instanceToUpdate, ModelState);
            if (!TryValidateModel(instanceToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await _instanceRepo.UpdateRecordAsync(instanceToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update vocab instance.");
            }
            return Ok(instanceToUpdate);
        }

        // DELETE: instances/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInstance(int id)
        {
            VocabInstance? instance = await _instanceRepo
                .GetRecordByIdAsync(id, qu => qu
                    .Include(i => i.Vocab));
            if (instance == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || instance.Vocab?.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }
            
            int rowsAffected = await _instanceRepo.DeleteRecordAsync(instance);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete vocab instance.");
            }

            return NoContent();
        }
    }
}
