using Spectre.Console.Cli;

namespace Chrono.Commands.Options;

public class StartStopOptions : CommandSettings
{
    [CommandArgument(0, "[Task]")]
    public string Task { get; set; }
    
    [CommandArgument(1, "[Project]")]
    public string Project { get; set; }
    
}