using MatthiWare.CommandLine;
using RemoteDispatcher.Infrastructure;
using RemoteDispatcher.Properties;
using System;
using System.Linq;
using System.Reflection;

namespace RemoteDispatcher.Infrastucture
{
    /// <summary>
    ///     Manages the parsing operations for the command line.
    /// </summary>
    public class ParsingManager
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="ParsingManager"/>.
        /// </summary>
        /// <param name="options">The custom parsing options to set.</param>
        public ParsingManager(CommandLineParserOptions options)
            => Parser = new CommandLineParser(options);

        /// <summary>
        ///     Gets the local parser for this manager.
        /// </summary>
        public CommandLineParser Parser { get; }

        /// <summary>
        ///     Adds a command to the initial parser.
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="Command{T}"/> to set.</typeparam>
        /// <param name="required">Whether the command is required or not.</param>
        /// <returns>The instance of this class.</returns>
        public ParsingManager AddCommand<TCommand>(bool required = false) where TCommand : Command<TCommand>, new()
        {
            var name = GetCommandName<TCommand>();
            var description = GetCommandDescription<TCommand>();

            Parser.AddCommand<TCommand>()
                .Name(name)
                .Description(description)
                .Required(required)
                .OnExecuting(Command<TCommand>.Instance.OnExecuting);

            return this;
        }

        /// <summary>
        ///     Adds a command to the initial parser.
        ///     This methods sets the execution callback as asynchronous.
        /// </summary>
        /// <typeparam name="TCommand">The type of <see cref="Command{T}"/> to set.</typeparam>
        /// <param name="required">Whether the command is required or not.</param>
        /// <returns>The instance of this class.</returns>
        public ParsingManager AddAsyncCommand<TCommand>(bool required = false) where TCommand : Command<TCommand>, new()
        {
            var name = GetCommandName<TCommand>();
            var description = GetCommandDescription<TCommand>();

            Parser.AddCommand<TCommand>()
                .Name(name)
                .Description(description)
                .Required(required)
                .OnExecutingAsync(Command<TCommand>.Instance.OnExecutingAsync);

            return this;
        }

        /// <summary>
        ///     Gets and returns the name of the specified type assuming that the required
        ///     <see cref="Annotations.NameAttribute"/> set provided.
        /// </summary>
        /// <typeparam name="T">The type to get the name for.</typeparam>
        /// <returns>
        ///     The name of the specified type.
        /// </returns>
        private static string GetCommandName<T>()
        {
            var attr = GetRequiredAttribute<Annotations.NameAttribute>(typeof(T));
            return attr.Name;
        }

        /// <summary>
        ///     Gets and returns the description of the specified type assuming that the required
        ///     <see cref="Annotations.DescriptionAttribute"/> set provided.
        /// </summary>
        /// <typeparam name="T">The type to get the description for.</typeparam>
        /// <returns>
        ///     The description of the specified type.
        /// </returns>
        private static string GetCommandDescription<T>()
        {
            var attr = GetRequiredAttribute<Annotations.DescriptionAttribute>(typeof(T));
            return attr.Description;
        }

        /// <summary>
        ///     Gets and return the required attribute of the specified type if any; otherwise, throws an exception.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the required attribute to look for.</typeparam>
        /// <param name="type">The type to check whether it has the required attribute or not.</param>
        /// <returns>
        ///     The required attribute of the specified type if any.
        /// </returns>
        private static TAttribute GetRequiredAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            var attrs = type.GetCustomAttributes<TAttribute>();

            // Throw an error if the specified type doesn't contain the required attribute..
            if (!attrs.Any())
                throw new InvalidOperationException(
                    Resources.RequiredAttributeError(typeof(TAttribute).FullName));

            return attrs.First();
        }
    }
}