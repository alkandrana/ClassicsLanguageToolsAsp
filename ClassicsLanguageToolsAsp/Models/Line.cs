using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Models;
public class Line
{
    public int Id { get; set; }
    public int Number { get; set; }
    [Required] public string Text { get; set; } = "";
    public string? Locus { get; set; } = "";
    public int OpusId { get; set; }
    public Opus? Opus { get; set; }
    public ICollection<VocabInstance> VocabReferences { get; set; } = new List<VocabInstance>();
}