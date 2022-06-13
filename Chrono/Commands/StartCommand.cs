using Chrono.Commands.Options;
using Spectre.Console.Cli;

namespace Chrono.Commands;

public class StartCommand : AsyncCommand<StartOptions>
{
    public override Task<int> ExecuteAsync(CommandContext context, StartOptions settings)
    {
        throw new NotImplementedException();
    }
}