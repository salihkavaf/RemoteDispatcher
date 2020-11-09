using MatthiWare.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemoteDispatcher.Commands;
using RemoteDispatcher.Infrastucture;
using RemoteDispatcher.Properties;

namespace RemoteDispatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .AddEnvironmentVariables(Resources.EnvPrefix)
                    .Build();

            RegisterServices(config)
                .GetService<ParserBuilder>()
                .Configure(manager =>
                {
                    manager.AddAsyncCommand<RepositoryDispatchCommand>();
                })
                .Build(args);
        }

        public static ServiceProvider RegisterServices(IConfiguration configuration)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton(new CommandLineParserOptions
                {
                    AppName = Resources.ApplicationName
                })
                .AddSingleton<ParsingManager>()
                .AddSingleton<ParserBuilder>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}