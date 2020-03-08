using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SeedCore.SpaService.Internal
{
    internal static class SpaServiceMiddleware
    {
        private const string LogCategoryName = "SeedCore.SpaService";
        private static TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(30);

        public static void Attach(
            ISeedSpaBuilder spaBuilder,
            string scriptName)
        {
            var attribute = spaBuilder.Assembly.GetCustomAttribute<SpaProjectAttribute>();
            var sourcePath = Path.Combine(attribute.Project, attribute.Path);
            var pkgManagerCommand = spaBuilder.Server.PkgManagerCommand;
            var devServerPort = spaBuilder.Server.DevServerPort;

            if (!Directory.Exists(sourcePath))
            {
                return;
            }

            if (string.IsNullOrEmpty(scriptName))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(scriptName));
            }

            var appBuilder = spaBuilder.ApplicationBuilder;
            var applicationStoppingToken = appBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;
            var logger = LoggerFinder.GetOrCreateLogger(appBuilder, LogCategoryName);
            var diagnosticSource = appBuilder.ApplicationServices.GetRequiredService<DiagnosticSource>();
            var portTask = StartCreateAppServerAsync(
                sourcePath,
                scriptName,
                pkgManagerCommand,
                devServerPort,
                spaBuilder.Server.SuccessRegx,
                spaBuilder.Assembly.GetName().Name,
                logger,
                diagnosticSource,
                applicationStoppingToken);

            var targetUriTask = portTask.ContinueWith(
                task => new UriBuilder("http", "localhost", task.Result).Uri);

            SeedSpaBuilderExtensions.UseProxyToSpaDevelopmentServer(spaBuilder, () =>
            {
                var timeout = spaBuilder.Options.StartupTimeout;
                return targetUriTask.WithTimeout(timeout,
                    $"The server did not start listening for requests " +
                    $"within the timeout period of {timeout.Seconds} seconds. " +
                    $"Check the log output for error information.");
            });
        }

        private static async Task<int> StartCreateAppServerAsync(
            string sourcePath,
            string scriptName,
            string pkgManagerCommand,
            int portNumber,
            string successRegx,
            string moduleName,
            ILogger logger,
            DiagnosticSource diagnosticSource,
            CancellationToken applicationStoppingToken)
        {
            if (portNumber == default(int))
            {
                portNumber = TcpPortFinder.FindAvailablePort();
            }
            logger.LogInformation($"Starting server on port {portNumber}...");

            var envVars = new Dictionary<string, string>
            {
                { "PORT", portNumber.ToString() },
                { "SEED_MODULE", moduleName }
            };

            var scriptRunner = new NodeScriptRunner(
                sourcePath, scriptName, null, envVars, pkgManagerCommand, diagnosticSource, applicationStoppingToken);
            scriptRunner.AttachToLogger(logger);

            using (var stdErrReader = new EventedStreamStringReader(scriptRunner.StdErr))
            {
                try
                {
                    await scriptRunner.StdOut.WaitForMatch(
                        new Regex(successRegx, RegexOptions.None, RegexMatchTimeout));
                }
                catch (EndOfStreamException ex)
                {
                    throw new InvalidOperationException(
                        $"The {pkgManagerCommand} script '{scriptName}' exited without indicating that the " +
                        $"create-react-app server was listening for requests. The error output was: " +
                        $"{stdErrReader.ReadAsString()}", ex);
                }
            }

            return portNumber;
        }

        public static async Task WithTimeout(this Task task, TimeSpan timeoutDelay, string message)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeoutDelay)))
            {
                task.Wait();
            }
            else
            {
                throw new TimeoutException(message);
            }
        }

        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeoutDelay, string message)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeoutDelay)))
            {
                return task.Result;
            }
            else
            {
                throw new TimeoutException(message);
            }
        }
    }
}
