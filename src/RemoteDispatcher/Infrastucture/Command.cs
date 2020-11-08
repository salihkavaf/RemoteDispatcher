using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteDispatcher.Infrastructure
{
    /// <summary>
    ///     Represents an abstraction API for a command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        ///     Represents the name for this command.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        ///     Represents the description for this command.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        ///     Configures the execution of the command.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnExecuting(object args);

        /// <summary>
        ///     Configures the execution of the command asynchronously.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The <see cref="Task"/> object representing the asynchronous operation.</returns>
        public abstract Task OnExecutingAsync(object args, CancellationToken cancellationToken);
    }
}