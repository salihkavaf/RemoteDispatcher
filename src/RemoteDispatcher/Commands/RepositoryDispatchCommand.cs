using MatthiWare.CommandLine.Core.Attributes;
using Microsoft.AspNetCore.WebUtilities;
using RemoteDispatcher.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADescription = MatthiWare.CommandLine.Core.Attributes.DescriptionAttribute;
using AName = MatthiWare.CommandLine.Core.Attributes.NameAttribute;

namespace RemoteDispatcher.Commands
{
    [Annotations.Name("dispatch-repository"), Annotations.Description("Triggers a Github action repository_dispatch event remotely.")]
    public class RepositoryDispatchCommand : Command<RepositoryDispatchCommand>
    {
        private const string Server = "https://api.github.com";
        private const string RelativeUri = "repos/{0}/{1}/dispatches";

        /// <summary>
        ///     Gets or sets the content type to accept in the request.
        /// </summary>
        [AName("a", "accept"), ADescription("The content type of the request.")]
        public string Accept { get; set; } = "application/vnd.github.v3+json";

        /// <summary>
        ///     Gets or sets the name of the repository name.
        /// </summary>
        [Required, AName("o", "owner"), ADescription("The owner or the organization name.")]
        public string Owner { get; set; }

        /// <summary>
        ///     Gets or sets the name of the target repository.
        /// </summary>
        [Required, AName("r", "repo"), ADescription("The name of the target repository.")]
        public string Repo { get; set; }

        /// <summary>
        ///     Gets or sets the name of the custom webhook event.
        /// </summary>
        [Required, AName("e", "event-type"), ADescription("Required.A custom webhook event name.")]
        public string EventType { get; set; }

        /// <summary>
        ///     Gets or sets the JSON payload with extra information about the event.
        /// </summary>
        [AName("p", "client-payload"), ADescription("JSON payload with extra information about the webhook event that your action or workflow may use.")]
        public string ClientPayload { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicating whether the target repository is private or not.
        /// </summary>
        [AName("p", "is-private"), ADescription("The flag indicating whether the target repo id private or not.")]
        public bool IsPrivate { get; set; }

        /// <inheritdoc />
        public override async Task OnExecutingAsync(object args, RepositoryDispatchCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            LogInfo(command);

            // Start watching execution time..
            var sw = new Stopwatch();
            sw.Start();
            var response = await ExecuteHttpPostAsync(command, cancellationToken);
            sw.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Repository is successfully dispatched.");
                Console.WriteLine($"Time Elapsed {sw.Elapsed.TotalSeconds}s");
                return;
            }
            await LogErrorsAsync(response, cancellationToken);
            Console.WriteLine($"Time Elapsed {sw.Elapsed.TotalSeconds}s");
        }

        /// <summary>
        ///     Prints out command properties.
        /// </summary>
        /// <param name="command">The command to print its properties out.</param>
        private static void LogInfo(RepositoryDispatchCommand command)
        {
            foreach (var property in command.GetType().GetProperties(BindingFlags.Public))
            {
                var value = property.GetValue(command);
                Console.WriteLine($"{property.Name}: {value}");
            }
        }

        /// <summary>
        ///     Executes an HTTP Post request, as an asynchronous operation.
        /// </summary>
        /// <param name="command">The command to get the data from.</param>
        /// <param name="cancellationToken">The token to check whether the operation should be canceled or not.</param>
        /// <returns>
        ///     The <see cref="Task"/> object that represents the asynchronous operation, containing the response message.
        /// </returns>
        private static async Task<HttpResponseMessage> ExecuteHttpPostAsync(RepositoryDispatchCommand command, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();

            // Set the request URI..
            var server = new Uri(Server);
            var relativePath = new Uri(string.Format(RelativeUri, command.Owner, command.Repo), UriKind.Relative);
            var fullUri = new Uri(server, relativePath);

            // Set the response content..
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(new
                {
                    event_type = command.EventType
                }),
                Encoding.UTF8,
                command.Accept);

            // Execute the request..
            return await client.PostAsync(fullUri, jsonContent, cancellationToken);
        }

        /// <summary>
        ///     Logs the errors in included in the specified response message, as an asynchronous operation.
        /// </summary>
        /// <param name="response">The response message to get the error messages from.</param>
        /// <param name="cancellationToken">The token to check whether the operation should be canceled or not.</param>
        /// <returns>The <see cref="Task"/> object that represents the asynchronous operation.</returns>
        private static async Task LogErrorsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var statusCode = (int)response.StatusCode;
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var message = await reader.ReadToEndAsync();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{ReasonPhrases.GetReasonPhrase(statusCode)} [{statusCode}]");
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}