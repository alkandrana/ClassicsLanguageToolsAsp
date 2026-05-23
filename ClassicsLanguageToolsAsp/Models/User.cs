using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class User : IdentityUser
{
    public ICollection<Vocab> VocabList { get; set; } = new List<Vocab>();
}