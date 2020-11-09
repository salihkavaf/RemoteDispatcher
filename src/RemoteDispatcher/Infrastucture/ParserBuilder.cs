using MatthiWare.CommandLine;
using RemoteDispatcher.Properties;
using System;

namespace RemoteDispatcher.Infrastucture
{
    public sealed class ParserBuilder
    {
        private readonly ParsingManager manager;

        public static readonly ParserBuilder Builder = new ParserBuilder();

        /// <summary>
        ///     Initializes a new instance of <see cref="ParserBuilder"/>.
        /// </summary>
        private ParserBuilder()
        {
            var options = new CommandLineParserOptions
            {
                AppName = Resources.ApplicationName,
                EnableHelpOption = true,
                HelpOptionName = "help",
                AutoPrintUsageAndErrors = true
            };
            manager = new ParsingManager(options);
        }

        /// <summary>
        ///     Configures the Parsing operations.
        /// </summary>
        /// <param name="action">The action to set configuration.</param>
        /// <returns>The current instance of the <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder Configure(Action<ParsingManager> action)
        {
            action(manager);
            return this;
        }

        /// <summary>
        ///     Build the <see cref="ParsingManager"/> along with specified configuration.
        /// </summary>
        public void Build(string[] args)
        {
            try
            {
                var result = manager.Parser.Parse(args);
                if (result.HasErrors)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.Error.WriteLine(error.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}