using Microsoft.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Models;
[Index(nameof(Code), IsUnique = true)]
public class Author
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Praenomen { get; set; }
    public string? Nomen { get; set; }
    public string? Cognomen { get; set; }
    
    public ICollection<Opus> Opera { get; set; } =  new List<Opus>();
}