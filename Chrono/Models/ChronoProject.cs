namespace Chrono.Models;

public class ChronoProject
{
    public string Name { get; set; }
    public List<ChronoTask> Tasks { get; set; } = new List<ChronoTask>();

    public ChronoProject(string name)
    {
        Name = name;
    }
    
}