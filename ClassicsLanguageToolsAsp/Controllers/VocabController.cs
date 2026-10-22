using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WritingSessionsAspApi.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers;

[Authorize]
[ApiController]
[Route("vocab")]
public class VocabController : ControllerBase
{
    private IRecordRepo<Vocab> _vocabRepo;
    private UserManager<ClassUser> _userManager;
    
    public VocabController(IRecordRepo<Vocab> vocabRepo, UserManager<ClassUser> userMng)
    {
        _vocabRepo = vocabRepo;
        _userManager = userMng;
    }

    [HttpGet]
    public async Task<IActionResult> GetVocab()
    {
        ClassUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }
        List<Vocab> vocab = await _vocabRepo.GetSelectRecordsAsync(v => v.CreatorId == currentUser.Id);
        return Ok(vocab);
    }

    [HttpGet]
    [Route("/vocab/language/{languageId}")]
    public async Task<IActionResult> GetVocabByLanguage(int languageId)
    {
        ClassUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }
        List<Vocab> vocab = await _vocabRepo
            .GetSelectRecordsAsync(v => v.LanguageId == languageId && 
                        v.CreatorId == currentUser.Id);
        return Ok(vocab);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneVocab(int id)
    {
        ClassUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }
        Vocab? vocab = await _vocabRepo
            .GetRecordByIdAsync(id, qu => qu
                .Include(v => v.Language)
            .Include(v => v.Creator));
        if (vocab == null)
        {
            return NotFound();
        } 
        
        if (vocab.CreatorId != currentUser.Id)
        {
            return Unauthorized();
        }
        return Ok(vocab);
    }

    [HttpPost]
    public async Task<IActionResult> AddVocab([FromBody] Vocab newVocab)
    {
        ClassUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }
        newVocab.Creator = currentUser;
        List<Vocab> duplicates = await _vocabRepo.GetSelectRecordsAsync(v => v.Lemma == newVocab.Lemma);
        if (duplicates.Count > 0)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Duplicate entry",
                Detail = "That vocab entry already exists.",
            });
        }
        int rowsAffected = await _vocabRepo.CreateRecordAsync(newVocab);
        if (rowsAffected == 0)
        {
            return Problem("Failed to create vocab.");
        }
        return CreatedAtAction(nameof(GetOneVocab), 
            new { id = newVocab.Id }, newVocab);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditVocab([FromRoute] int id, [FromBody] JsonPatchDocument<Vocab> vocabData)
    {
        Vocab? vocab = await _vocabRepo.GetRecordByIdAsync(id);
        if (vocab == null)
        {
            return NotFound();
        }
        vocabData.ApplyTo(vocab, ModelState);
        if (!TryValidateModel(vocab) || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int rowsAffected = await _vocabRepo.UpdateRecordAsync(vocab);
        if (rowsAffected == 0)
        {
            return Problem("Failed to update vocab.");
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVocab(int id)
    {
        ClassUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }
        
        Vocab? vocab = await _vocabRepo
            .GetRecordByIdAsync(id);
        if (vocab == null)
        {
            return NotFound();
        }

        if (vocab.CreatorId != currentUser.Id)
        {
            return Unauthorized();
        }
        
        int rowsAffected = await _vocabRepo.DeleteRecordAsync(vocab);
        if (rowsAffected == 0)
        {
            return Problem("Failed to delete vocab.");
        }
        return NoContent();
    }
}

