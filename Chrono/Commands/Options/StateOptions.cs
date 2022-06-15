using Spectre.Console.Cli;

namespace Chrono.Commands.Options;

public class StateOptions : CommandSettings
{
    [CommandArgument(0, "[Action]")]
    public string Action { get; set; }
    
    
}