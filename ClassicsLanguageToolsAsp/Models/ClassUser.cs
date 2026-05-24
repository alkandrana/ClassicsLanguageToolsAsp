using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class ClassUser : IdentityUser
{
    public ICollection<Vocab> VocabList { get; set; } = new List<Vocab>();
}