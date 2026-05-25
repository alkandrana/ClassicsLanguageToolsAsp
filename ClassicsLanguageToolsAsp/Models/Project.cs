using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class Project
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string? Description { get; set; }
    public string Work { get; set; }
    public DateTime StartDate { get; set; }
    public DateOnly? Deadline { get; set; }
    public IdentityUser? Creator { get; set; }
    
    public ICollection<VocabInstance> VocabList { get; set; } = new List<VocabInstance>();
}