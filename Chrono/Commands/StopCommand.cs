using Chrono.Commands.Options;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StopCommand : AsyncCommand
{
    private ILogger<StopCommand> _logger;
    private IChronoState _chronoState;

    public StopCommand(ILogger<StopCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override Task<int> ExecuteAsync(CommandContext context)
    {
        throw new NotImplementedException();
    }
}