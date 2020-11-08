using MatthiWare.CommandLine;
using RemoteDispatcher.Properties;

namespace RemoteDispatcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new CommandLineParserOptions
            {
                AppName = Resources.ApplicationName
            };

            var parser = new CommandLineParser(options);
            parser.AddCommand()
                .OnExecuting
        }
    }
}