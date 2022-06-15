using Chrono.Commands.Options;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StopCommand : AsyncCommand<StartStopOptions>
{
    private ILogger<StopCommand> _logger;
    private IChronoState _chronoState;

    public StopCommand(ILogger<StopCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, StartStopOptions settings)
    {
        if(settings.Task == null || settings.Project == null)
            await RunTaskStopWizardAsync();
        else
            await RunTaskStopAsync(settings.Task, settings.Project);

        return 0;
    }

    private async Task RunTaskStopAsync(string taskName, string projectName)
    {
        var tasks = await _chronoState.GetChronoTasksAsync();
        var projects = await _chronoState.GetChronoProjectsAsync();
        
        // Check if project exist and create otherwise
        if (projects.All(p => p.Name != projectName))
            await _chronoState.AddProjectAsync(new ChronoProject(projectName));

        // Check if task exists and create otherwise
        if (tasks.All(t => t.Name != taskName))
            await _chronoState.AddTaskAsync(new ChronoTask(taskName, projectName), projectName);
        
        // Start the task
        await _chronoState.StopTaskAsync(taskName);
        AnsiConsole.Markup($"[green]Task '{taskName}' was stopped in project '{projectName}'[/]");
    }
    
    private async Task RunTaskStopWizardAsync()
    {
        var tasks = await _chronoState.GetChronoTasksAsync();
        
        // Get the task name
        var selectedTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What task would you like to stop?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more tasks)[/]")
                .AddChoices(tasks.Where(t => t.RunningSince != null).Select(t => t.Name)));

        await _chronoState.StopTaskAsync(selectedTask);
        AnsiConsole.Markup($"[green]Task '{selectedTask}' was stopped.[/]");
    }
}