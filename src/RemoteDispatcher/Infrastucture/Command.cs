using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteDispatcher.Infrastructure
{
    /// <summary>
    ///     Represents an abstraction API for a command.
    /// </summary>
    public abstract class Command<T> where T : Command<T>, new()
    {
        public readonly static Command<T> Instance = new T();

        /// <summary>
        ///     Configures the execution of the command.
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnExecuting(object args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Configures the execution of the command asynchronously.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The <see cref="Task"/> object representing the asynchronous operation.</returns>
        public virtual Task OnExecutingAsync(object args, T command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}