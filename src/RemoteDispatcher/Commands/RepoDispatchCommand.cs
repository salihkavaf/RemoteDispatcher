using MatthiWare.CommandLine.Abstractions.Command;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using RemoteDispatcher.Properties;
using System;
using System.Diagnostics;
using System.Dynamic;
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

        private const string Server = "https://api.github.com";
        private const string RelativeUri = "repos/{0}/{1}/dispatches";
        private readonly IConfiguration configuration;

        /// <inheritdoc />
        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            builder.Name(Resources.RepoDispatchName);
            builder.Description(Resources.RepoDispatchDescription);
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
        /// <param name="options">The provided options to use in configuration.</param>
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
        /// <param name="options">The provided options to use in configuration.</param>
        /// <param name="cancellationToken">The token to check whether the operation should be canceled or not.</param>
        /// <returns>
        ///     The <see cref="Task"/> object that represents the asynchronous operation, containing the response message.
        /// </returns>
        private async Task<HttpResponseMessage> ExecuteHttpPostAsync(RepoDispatchOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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

            // Load the extra data if provided..
            var client_payload = await LoadDataAsync(options, cancellationToken);
            

            // Set the response content..
            var json = JsonSerializer.Serialize(new { event_type = options.EventType, client_payload });
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
            cancellationToken.ThrowIfCancellationRequested();

            var statusCode = (int)response.StatusCode;
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var message = await reader.ReadToEndAsync();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{ReasonPhrases.GetReasonPhrase(statusCode)} [{statusCode}]");
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Loads and returns the extra data if provided.
        /// </summary>
        /// <param name="options">The options object to get data or data source from.</param>
        /// <param name="cancellationToken">The token to check whether the operation should be canceled or not.</param>
        /// <returns>The extra data if provided; otherwise, null.</returns>
        private static async Task<object> LoadDataAsync(RepoDispatchOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Return null if no data were provided..
            if (options.ClientPayload == null && options.DataFile == null)
                return JsonSerializer.Deserialize<dynamic>("{}");

            string data = null;
            // Load the data file if provided..
            if (options.DataFile != null)
            {
                data = await File.ReadAllTextAsync(options.DataFile, Encoding.UTF8, cancellationToken);

                // Throw an exception if the provided data was null or empty.
                if (string.IsNullOrEmpty(data.Trim()))
                {
                    throw new InvalidOperationException(Resources.ErrorRepoDispatchInvalidData);
                }
            }
            else if (options.ClientPayload != null)
            {
                data = options.ClientPayload;
            }
            return JsonSerializer.Deserialize<dynamic>(data);
        }
    }
}