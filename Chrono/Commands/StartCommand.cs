using Chrono.Commands.Options;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StartCommand : AsyncCommand<StartStopOptions>
{
    private ILogger<StartCommand> _logger;
    private IChronoState _chronoState;

    public StartCommand(ILogger<StartCommand> logger, IChronoState chronoState)
    {
        _logger = logger;
        _chronoState = chronoState;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, StartStopOptions settings)
    {
        if(settings.Task == null || settings.Project == null)
            await RunTaskStartWizardAsync();
        else
            await RunTaskStartAsync(settings.Task, settings.Project);

        return 0;
    }

    private async Task RunTaskStartAsync(string taskName, string projectName)
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
        await _chronoState.StartTaskAsync(taskName);
        AnsiConsole.Markup($"[green]Task '{taskName}' was started in project '{projectName}'[/]");
    }
    
    private async Task RunTaskStartWizardAsync()
    {
        var tasks = await _chronoState.GetChronoTasksAsync();
        
        // Get the task name
        var selectedTask = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What task would you like to start?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more tasks)[/]")
                .AddChoices(tasks.Select(t => t.Name).Prepend("New Task")));

        if (selectedTask != "New Task")
        {
            await _chronoState.StartTaskAsync(selectedTask);
            return;
        }
        
        var taskName = AnsiConsole.Ask<string>("What's the task name?");

        var projects = await _chronoState.GetChronoProjectsAsync();
        
        var selectedProject = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which project should the task run in?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                .AddChoices(projects.Select(p => p.Name).Prepend("New Project")));

        if (selectedProject == "New Project")
        {
            selectedProject = AnsiConsole.Ask<string>("What's the new project name?");
            await _chronoState.AddProjectAsync(new ChronoProject(selectedProject));
        }

        await _chronoState.AddTaskAsync(new ChronoTask(taskName, selectedProject), selectedProject);
        AnsiConsole.Markup($"[green]Task '{taskName}' was started in project '{selectedProject}'[/]");
    }
}