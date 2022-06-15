using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using Chrono.Models;
using Chrono.Storage.Abstractions;
using Chrono.Storage.Exceptions;
using Microsoft.Extensions.Logging;

namespace Chrono.Storage;

public class LocalChronoState : IChronoState
{
    private readonly ILogger<LocalChronoState> _logger;
    private readonly List<ChronoProject> _state;

    public LocalChronoState(ILogger<LocalChronoState> logger)
    {
        _logger = logger;
        
        if(!File.Exists("chrono.state"))
            File.WriteAllText("chrono.state", "[]");

        try
        {
            _state = JsonSerializer.Deserialize<List<ChronoProject>>(File.ReadAllText("chrono.state"));
        }
        catch (JsonException)
        {
            _logger.LogWarning("Failed to parse Chrono state. It is either corrupted or this is your first run?");
        }

        _state ??= new List<ChronoProject>();
    }

    private void StoreState()
    {
        // Store a backup
        File.Delete("chrono.state.backup");
        File.Copy("chrono.state", "chrono.state.backup");
        
        // Write the new state
        File.WriteAllText("chrono.state", JsonSerializer.Serialize(_state, new JsonSerializerOptions(){WriteIndented = true}));
    }


    public Task<List<ChronoProject>> GetChronoProjectsAsync()
        => Task.FromResult(_state.ToList());

    public Task<List<ChronoTask>> GetChronoTasksAsync()
        => Task.FromResult(_state.SelectMany(project => project.Tasks).ToList());
    

    public Task<List<ChronoTask>> GetChronoTasksAsync(string projectName)
    {
        // Find the project
        var project = _state.FirstOrDefault(p => p.Name == projectName);

        // Throw if not found
        if (project == null)
            throw new ChronoProjectNotFoundException();
        
        // Return project tasks
        return Task.FromResult(project.Tasks);
    }


    public Task AddTaskAsync(ChronoTask task, string projectName)
    {
        // Find the project
        var project = _state.FirstOrDefault(p => p.Name == projectName);

        // Throw if not found
        if (project == null)
            throw new ChronoProjectNotFoundException();
        
        // Add task to project
        project.Tasks.Add(task);
        
        StoreState();
        return Task.CompletedTask;
    }

    public Task StartTaskAsync(string taskName)
    {
        // Find the task
        var task = _state.SelectMany(p => p.Tasks).FirstOrDefault(t => t.Name == taskName);
        
        // Throw if task doesn't exist
        if (task == null)
            throw new ChronoTaskNotFoundException();

        // Throw if task is already running
        if (task.RunningSince != null)
            throw new ChronoTaskAlreadyRunningException();

        // Start task
        task.RunningSince = DateTime.Now;
        
        StoreState();
        return Task.CompletedTask;
    }

    public Task StopTaskAsync(string taskName)
    {
        // Find the task
        var task = _state.SelectMany(p => p.Tasks).FirstOrDefault(t => t.Name == taskName);
        
        // Throw if task doesn't exist
        if (task == null)
            throw new ChronoTaskNotFoundException();

        // Throw if task is already running
        if (task.RunningSince == null)
            throw new ChronoTaskNotRunningException();

        // Stop task
        task.TotalTimeSeconds += (float)(DateTime.Now - ((DateTime)task.RunningSince)).TotalSeconds;
        task.RunningSince = null;
        
        StoreState();
        return Task.CompletedTask;
    }

    public Task RemoveTaskAsync(string taskName)
    {
        // Find the project
        var project = _state.FirstOrDefault(p => p.Tasks.Any(t => t.Name == taskName));

        // Throw if no associated project is found
        if (project == null)
            throw new ChronoTaskHasNoProjectException();

        // Remove the task
        project.Tasks.Remove(project.Tasks.First(t => t.Name == taskName));
        
        StoreState();
        return Task.CompletedTask;
    }

    public Task ClearStateAsync()
    {
        // Clear state
        _state.Clear();
        StoreState();

        return Task.CompletedTask;
    }

    public Task AddProjectAsync(ChronoProject project)
    {
        // Throw if project with same name already exists
        if (_state.Any(p => p.Name == project.Name))
            throw new ChronoProjectExistsException();

        // Add the new project
        _state.Add(project);
        
        StoreState();
        return Task.CompletedTask;
    }

    public Task RemoveProjectAsync(string projectName)
    {
        // Find the project
        var project = _state.FirstOrDefault(p => p.Name == projectName);

        // Throw if not found
        if (project == null)
            throw new ChronoProjectNotFoundException();

        _state.Remove(project);
        
        StoreState();
        return Task.CompletedTask;
    }
}