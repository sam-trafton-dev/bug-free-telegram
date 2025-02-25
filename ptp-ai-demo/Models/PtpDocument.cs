namespace ptp_ai_demo.Models;

public class PtpDocument
{
    public string Title { get; set; }
    public List<Section> Sections { get; set; }
    public string Summary { get; set; }
    
}

public class Section
{
    public string Header { get; set; }
    public List<string> Content { get; set; }
}