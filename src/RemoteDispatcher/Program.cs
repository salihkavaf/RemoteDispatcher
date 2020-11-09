using Microsoft.Extensions.Configuration;
using RemoteDispatcher.Commands;
using RemoteDispatcher.Infrastucture;
using RemoteDispatcher.Properties;
using System;

namespace RemoteDispatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder =
                new ConfigurationBuilder()
                    .AddEnvironmentVariables();

            var config = builder.Build();

            var env = config["CLIENT_ID"];

            ParserBuilder.Builder
                .Configure(manager =>
                {
                    manager.AddAsyncCommand<RepositoryDispatchCommand>();
                })
                .Build(args);
        }
    }
}