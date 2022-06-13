using Chrono.Commands;
using Chrono.DI;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;


// Prepare services
var services = new ServiceCollection();

// Define app
var app = new CommandApp(new TypeRegistrar(services));
app.Configure(config =>
{
    config.AddCommand<StartCommand>("start");
    config.AddCommand<StopCommand>("stop");
});

// Run app
return await app.RunAsync(args);