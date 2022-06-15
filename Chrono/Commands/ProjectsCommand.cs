using Chrono.Commands.Options;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class ProjectsCommand : AsyncCommand<ProjectsOptions>
{
    private ILogger<ProjectsCommand> _logger;
    private IChronoState _chronoState;

    public ProjectsCommand(ILogger<ProjectsCommand> logger, IChronoState chronoState)
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
                await PrintProjectsAsync();
                break;
            
            case "add":
                await AddProjectAsync(settings.Name);
                break;
            
            case "rm":
            case "remove":
                await RemoveProjectAsync(settings.Name);
                break;
            
            default:
                HandleUnknownAction(settings.Action);
                break;
        }

        return 0;
    }

    private async Task AddProjectAsync(string projectName)
    {
        if(projectName == null)
            projectName = AnsiConsole.Ask<string>("What's the project name?");
        
        await _chronoState.AddProjectAsync(new ChronoProject(projectName));
        AnsiConsole.Markup("[green]The project has been created.[/]");
    }

    private async Task RemoveProjectAsync(string projectName)
    {
        if (projectName == null)
        {
            var projects = await _chronoState.GetChronoProjectsAsync();
        
            projectName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which project should be removed?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                    .AddChoices(projects.Select(p => p.Name)));
        }
        
        await _chronoState.RemoveProjectAsync(projectName);
        AnsiConsole.Markup($"[green]The project has been removed.[/]");
    }
    
    private async Task PrintProjectsAsync()
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Project");
        table.AddColumn("Tasks");
        table.AddColumn("Total Time");

        // Add some rows
        foreach (var project in await _chronoState.GetChronoProjectsAsync())
            table.AddRow(project.Name, "1, 2, 3", $"{project.Tasks.Sum(t => t.TotalTimeSeconds)} seconds");
        
        // Render the table to the console
        AnsiConsole.Write(table);
    }
    
    private void HandleUnknownAction(string action)
    {
        _logger.LogError("Failed to execute '{action}'. This is not a supported 'projects' action.", action);
    }
}