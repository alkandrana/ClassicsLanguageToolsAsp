using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Models;

[Index(nameof(Code), nameof(AuthorId), IsUnique = true)]
public class Opus
{
    public int  Id { get; set; }
    [Required] public string Code { get; set; }
    [Required] public string Title { get; set; }
    public int LanguageId { get; set; }
    public Language? Language { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    
    public ICollection<Line> Lines { get; set; } =  new List<Line>();
}