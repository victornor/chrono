using Chrono.Commands.Options;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StartCommand : AsyncCommand<StartOptions>
{
    private ILogger<StartCommand> _logger;
    private IChronoState _chronoState;

    public StartCommand(ILogger<StartCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override Task<int> ExecuteAsync(CommandContext context, StartOptions settings)
    {
        throw new NotImplementedException();
    }
}