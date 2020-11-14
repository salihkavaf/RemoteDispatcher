using MatthiWare.CommandLine.Core.Attributes;

namespace RemoteDispatcher.Commands
{
    public class RepoDispatchOptions
    {
        /// <summary>
        ///     Gets or sets the content type to accept in the request.
        /// </summary>
        [Name("a", "accept"), Description("The content type of the request.")]
        public string Accept { get; set; } = "application/vnd.github.v3+json";

        /// <summary>
        ///     Gets or sets the name of the repository name.
        /// </summary>
        [Required, Name("o", "owner"), Description("The owner or the organization name.")]
        public string Owner { get; set; }

        /// <summary>
        ///     Gets or sets the name of the target repository.
        /// </summary>
        [Required, Name("r", "repo"), Description("The name of the target repository.")]
        public string Repo { get; set; }

        /// <summary>
        ///     Gets or sets the name of the custom webhook event.
        /// </summary>
        [Required, Name("e", "event-type"), Description("Required.A custom webhook event name.")]
        public string EventType { get; set; }

        /// <summary>
        ///     Gets or sets the JSON payload with extra information about the event.
        /// </summary>
        [Name("d", "client-payload"), Description("JSON payload with extra information about the webhook event that your action or workflow may use.")]
        public string ClientPayload { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicating whether the target repository is private or not.
        /// </summary>
        [Name("p", "is-private"), Description("The flag indicating whether the target repo id private or not.")]
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Gets or sets the path to the file that contains the client payload data to load.
        /// </summary>
        [Name("f", "data-file"), Description("The path to the JSON file that contains the client payload data to load.")]
        public string DataFile { get; set; }
    }
}