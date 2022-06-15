using Chrono.Models;

namespace Chrono.Storage.Abstractions;

public interface IChronoState
{
    /* Projects */
    Task<List<ChronoProject>> GetChronoProjectsAsync();
    Task AddProjectAsync(ChronoProject project);
    Task RemoveProjectAsync(string projectName);
    
    /* Tasks */
    Task<List<ChronoTask>> GetChronoTasksAsync();
    Task<List<ChronoTask>> GetChronoTasksAsync(string projectName);
    Task AddTaskAsync(ChronoTask task, string projectName);
    Task StartTaskAsync(string taskName);
    Task StopTaskAsync(string taskName);
    Task RemoveTaskAsync(string taskName);
    
    /* State */
    Task ClearStateAsync();
}