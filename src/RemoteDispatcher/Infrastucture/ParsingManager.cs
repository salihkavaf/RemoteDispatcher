using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Core.Attributes;
using RemoteDispatcher.Infrastructure;
using System;
using System.Linq;
using System.Reflection;

namespace RemoteDispatcher.Infrastucture
{
    public class ParsingManager
    {
        private readonly CommandLineParser parser;

        public ParsingManager()
            => parser = new CommandLineParser();

        public ParsingManager(CommandLineParserOptions options)
            => parser = new CommandLineParser(options);

        public ParsingManager AddCommand<TCommand>() where TCommand : Command, new()
        {
            parser.AddCommand<TCommand>()
                .Name(GetCommandName<TCommand>())
                .Description(GetCommandDescription<TCommand>())
                .OnExecuting(TCommand) 

            return this;
        }

        private string GetCommandName<TCommand>() where TCommand : Command
        {
            var attr = GetCustomAttribute<NameAttribute>(typeof(TCommand));
            return attr.ShortName;
        }

        private string GetCommandDescription<TCommand>() where TCommand : Command
        {
            var attr = GetCustomAttribute<DescriptionAttribute>(typeof(TCommand));
            return attr.Description;
        }

        private TAttribute GetCustomAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            var attrs = type.GetCustomAttributes<TAttribute>();
            return attrs.FirstOrDefault();
        }

    }
}