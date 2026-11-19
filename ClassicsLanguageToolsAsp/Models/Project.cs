using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class Project
{
    public int Id { get; set; }
    public string Label { get; set; } = "";
    public string? Description { get; set; }
    public int OpusId { get; set; }
    public Opus? Opus { get; set; }
    public int? LastLineId { get; set; }
    public Line? LastLine { get; set; }
    public DateTime StartDate { get; set; }
    public DateOnly? Deadline { get; set; }
    public string? CreatorId { get; set; } = "";
    public ClassUser? Creator { get; set; }

    public ICollection<VocabInstance> VocabList { get; set; } = new List<VocabInstance>();
    public ICollection<Comment> Comments { get; set; } = [];
}