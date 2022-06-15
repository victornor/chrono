using Chrono.Commands.Options;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StateCommand : AsyncCommand<StateOptions>
{
    private ILogger<StateCommand> _logger;
    private IChronoState _chronoState;

    public StateCommand(ILogger<StateCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, StateOptions settings)
    {
        switch (settings.Action)
        {
            case "clear":
                await _chronoState.ClearStateAsync();
                AnsiConsole.Markup($"[green]Chrono state has been cleared. A backup is available (chrono.state.backup)[/]");
                break;

            default:
                HandleUnknownAction(settings.Action);
                break;
        }

        return 0;
    }

    private void HandleUnknownAction(string action)
    {
        _logger.LogError("Failed to execute '{action}'. This is not a supported action, please use 'clear' instead",
            action);
    }
}
