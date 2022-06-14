using Chrono.Commands.Options;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class AddCommand : AsyncCommand<AddOptions>
{
    private ILogger<AddCommand> _logger;
    private IChronoState _chronoState;

    public AddCommand(ILogger<AddCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, AddOptions settings)
    {
        switch (settings.Type)
        {
            case "project":
                await _chronoState.AddProjectAsync(new ChronoProject(settings.Name));
                break;
        }

        return 0;
    }
}