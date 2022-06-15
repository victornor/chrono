using Chrono.Commands.Options;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class TasksCommand : AsyncCommand<ProjectsOptions>
{
    private ILogger<TasksCommand> _logger;
    private IChronoState _chronoState;

    public TasksCommand(ILogger<TasksCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, ProjectsOptions settings)
    {
        switch (settings.Action)
        {
            case null:
            case "list":
                await PrintTasksAsync();
                break;
            
            case "rm":
            case "remove":
                await RemoveTaskAsync(settings.Name);
                break;
            
            default:
                HandleUnknownAction(settings.Action);
                break;
        }

        return 0;
    }

    private async Task RemoveTaskAsync(string taskName)
    {
        if (taskName == null)
        {
            var tasks = await _chronoState.GetChronoTasksAsync();
        
            taskName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which task should be removed?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                    .AddChoices(tasks.Select(p => p.Name)));
        }
        
        await _chronoState.RemoveProjectAsync(taskName);
        AnsiConsole.Markup($"[green]The project has been removed.[/]");
    }
    
    private async Task PrintTasksAsync()
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Task");
        table.AddColumn("Project");
        table.AddColumn("Total Time");

        // Add some rows
        foreach (var task in await _chronoState.GetChronoTasksAsync())
            table.AddRow(task.Name, task.ProjectName, $"{task.TotalTimeSeconds} seconds");
        
        // Render the table to the console
        AnsiConsole.Write(table);
    }
    
    private void HandleUnknownAction(string action)
    {
        _logger.LogError("Failed to execute '{action}'. This is not a supported 'projects' action.", action);
    }
}