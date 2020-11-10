using MatthiWare.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemoteDispatcher.Commands;
using RemoteDispatcher.Properties;
using System;

namespace RemoteDispatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = RegisterServices();
            var options = new CommandLineParserOptions
            {
                AppName = Resources.ApplicationName
            };

            var parser = new CommandLineParser(options, services);

            parser.RegisterCommand<RepoDispatchCommand, RepoDispatchOptions>();

            var result = parser.Parse(args);
            if (result.HasErrors)
            {
                foreach (var error in result.Errors)
                {
                    Console.Error.WriteLine(error);
                }
            }
        }

        public static IServiceCollection RegisterServices()
        {
            var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables(Resources.EnvPrefix)
                    .Build();

            return new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration);
        }
    }
}