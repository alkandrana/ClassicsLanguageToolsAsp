using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class ClassUser : IdentityUser
{
    public string? Name { get; set; }
    public ICollection<Vocab> VocabList { get; set; } = new List<Vocab>();
}