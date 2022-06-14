using Spectre.Console.Cli;

namespace Chrono.Commands.Options;

public class ShowOptions : CommandSettings
{
    [CommandArgument(0, "[Filter]")]
    public string Filter { get; set; }
    
    
}