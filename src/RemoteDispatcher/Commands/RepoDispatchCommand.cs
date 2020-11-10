using MatthiWare.CommandLine.Abstractions.Command;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteDispatcher.Commands
{
    public class RepoDispatchCommand : Command<object, RepoDispatchOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RepoDispatchCommand"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>, used to access the application settings.</param>
        public RepoDispatchCommand(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private const string Name = "dispatch-repo";
        private const string Description = "Triggers a Github action repository_dispatch event remotely.";

        private const string Server = "https://api.github.com";
        private const string RelativeUri = "repos/{0}/{1}/dispatches";
        private readonly IConfiguration configuration;

        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            builder.Name(Name);
            builder.Description(Description);
            builder.Required(false);
        }

        /// <inheritdoc />
        public override async Task OnExecuteAsync(object args, RepoDispatchOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            LogInfo(options);

            // Start watching execution time..
            var sw = new Stopwatch();
            sw.Start();
            var response = await ExecuteHttpPostAsync(options, cancellationToken);
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
        private void LogInfo(RepoDispatchOptions options)
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Public))
            {
                var value = property.GetValue(options);
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
        private async Task<HttpResponseMessage> ExecuteHttpPostAsync(RepoDispatchOptions options, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();

            // Set the request URI..
            var server = new Uri(Server);
            var relativePath = new Uri(string.Format(RelativeUri, options.Owner, options.Repo), UriKind.Relative);
            var fullUri = new Uri(server, relativePath);

            var assembly = Assembly.GetExecutingAssembly().GetName();
            var appName = assembly.Name;
            var version = assembly.Version.ToString();

            // Set request headers..
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(appName, version));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(options.Accept));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(Encoding.UTF8.BodyName));

            if (options.IsPrivate)
            {
                var section = configuration.GetSection("TOKEN");
                if (section == null || string.IsNullOrEmpty(section.Value.Trim()))
                    throw new InvalidDataException();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", section.Value);
            }

            // Set the response content..
            var json = JsonSerializer.Serialize(new { event_type = options.EventType });
            var jsonContent = new StringContent(json);

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