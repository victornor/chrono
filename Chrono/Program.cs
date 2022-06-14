using Chrono.Commands;
using Chrono.DI;
using Chrono.Storage;
using Chrono.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;


// Prepare services
var services = new ServiceCollection();
services.AddSingleton<IChronoState, LocalChronoState>();

// Define app
var app = new CommandApp(new TypeRegistrar(services));
app.Configure(config =>
{
    config.AddCommand<StartCommand>("start");
    config.AddCommand<StopCommand>("stop");
});

// Run app
return await app.RunAsync(args);