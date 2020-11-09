using Microsoft.Extensions.Configuration;
using System;

namespace RemoteDispatcher.Infrastucture
{
    public sealed class ParserBuilder
    {
        #region Private Members

        private readonly ParsingManager manager;
        private readonly IConfiguration configuration;

        #endregion Private Members

        /// <summary>
        ///     Initializes a new instance of <see cref="ParserBuilder"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>, used to access the application settings.</param>
        /// <param name="manager">The parsing manager to set the command line parsing structure.</param>
        public ParserBuilder(
            IConfiguration configuration,
            ParsingManager manager)
        {
            this.configuration = configuration;
            this.manager = manager;
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