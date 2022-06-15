using Chrono.Commands.Options;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class ShowCommand : AsyncCommand<ShowOptions>
{
    private ILogger<ShowCommand> _logger;
    private IChronoState _chronoState;

    public ShowCommand(ILogger<ShowCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, ShowOptions settings)
    {
        switch (settings.Filter)
        {
            case "tasks":
                await PrintTasksAsync();
                break;
            
            default:
                HandleUnknownFilter(settings.Filter);
                break;
        }

        return 0;
    }

    

    private async Task PrintTasksAsync()
    {
        
    }

    private void HandleUnknownFilter(string filter)
    {
        _logger.LogError("Failed to show '{filter}'. This is not a supported filter, please use 'projects' or 'tasks' instead", filter);
    }
}