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
    private readonly ObservableCollection<ChronoProject> _state;

    public LocalChronoState(ILogger<LocalChronoState> logger)
    {
        _logger = logger;
        
        if(!File.Exists("chrono.state"))
            File.WriteAllText("chrono.state", "[]");

        try
        {
            _state = new ObservableCollection<ChronoProject>(JsonSerializer.Deserialize<List<ChronoProject>>(File.ReadAllText("chrono.state")));
        }
        catch (JsonException)
        {
            _logger.LogWarning("Failed to parse Chrono state. It is either corrupted or this is your first run?");
        }

        _state ??= new ObservableCollection<ChronoProject>();
        _state.CollectionChanged += StoreChangedState;
    }

    private void StoreChangedState(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Store a backup
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
        return Task.CompletedTask;
    }

    public Task AddProjectAsync(ChronoProject project)
    {
        // Throw if project with same name already exists
        if (_state.Any(p => p.Name == project.Name))
            throw new ChronoProjectExistsException();

        // Add the new project
        _state.Add(project);
        return Task.CompletedTask;
    }
}