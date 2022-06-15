using Spectre.Console.Cli;

namespace Chrono.Commands.Options;

public class ProjectsOptions : CommandSettings
{
    [CommandArgument(0, "[Action]")]
    public string Action { get; set; }
    
    [CommandArgument(1, "[Name]")]
    public string Name { get; set; }
}