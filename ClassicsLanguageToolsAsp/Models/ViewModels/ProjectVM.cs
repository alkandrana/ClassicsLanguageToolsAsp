namespace ClassicsLanguageToolsAsp.Models.ViewModels;

public class ProjectVM
{
    public string Code { get; set; } // Code of the Work to be studied
    public string Title { get; set; } // Title of the Work to be studied
    public string AuthorName { get; set; } // Name of the Author
    public Dictionary<string, Dictionary<int, string>> Lines { get; set; } = new Dictionary<string, Dictionary<int, string>>();
    public List<VocabInstance> Vocab { get; set; } = new List<VocabInstance>();
}
