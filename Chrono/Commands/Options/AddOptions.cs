using Spectre.Console.Cli;

namespace Chrono.Commands.Options;

public class AddOptions : CommandSettings
{
    [CommandArgument(0, "[Type]")]
    public string Type { get; set; }
    
    [CommandArgument(1, "[Name]")]
    public string Name { get; set; }
}