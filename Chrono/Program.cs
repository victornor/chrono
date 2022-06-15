using Chrono.Commands;
using Chrono.DI;
using Chrono.Storage;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;


// Prepare services
var services = new ServiceCollection();
services.AddSingleton<IChronoState, LocalChronoState>();
services.AddLogging(builder => builder.AddConsole());

// Define app
var app = new CommandApp(new TypeRegistrar(services));
app.Configure(config =>
{
    config.AddCommand<StartCommand>("start");
    config.AddCommand<StopCommand>("stop");
    config.AddCommand<ProjectsCommand>("projects");
    config.AddCommand<StateCommand>("state");
    config.AddCommand<TasksCommand>("tasks");
});

// Run app
return await app.RunAsync(args);