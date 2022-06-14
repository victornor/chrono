using Chrono.Models;

namespace Chrono.Storage.Abstractions;

public interface IChronoState
{
    Task<List<ChronoProject>> GetChronoProjectsAsync();
    Task<List<ChronoTask>> GetChronoTasksAsync();
    Task<List<ChronoTask>> GetChronoTasksAsync(string projectName);

    Task AddTaskAsync(ChronoTask task, string projectName);
    Task AddProjectAsync(ChronoProject project);
}