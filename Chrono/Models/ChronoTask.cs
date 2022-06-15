namespace Chrono.Models;

public class ChronoTask
{
    public string Name { get; set; }
    public string ProjectName { get; set; }
    public float TotalTimeSeconds { get; set; }
    public DateTime? RunningSince { get; set; }

    public ChronoTask(string name, string projectName)
    {
        Name = name;
        ProjectName = projectName;
    }
}