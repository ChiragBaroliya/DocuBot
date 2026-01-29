using System;
using System.IO;
using Serilog;

namespace DocuBot.AI
{
    internal class Program
    {
        static int Main(string[] args)
        {
            // Cross-platform log path
            var logDir = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory) ?? "/", "Logs", "DocuBot.GitAgent");
            var logFile = Path.Combine(logDir, "git-agent-.log");

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File(
                        path: logFile,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                        shared: true
                    )
                    .CreateLogger();
            }
            catch
            {
                // Logging failures should not break the app
            }

            Log.Information("Application start");

            int exitCode = 1;
            try
            {
                if (args is null || args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("❌ Usage: DocuBot.AI <commit-message-file>");
                    Log.Error("No commit message file path provided.");
                    return 1;
                }

                string filePath = args[0] ?? string.Empty;
                Log.Information("Commit message file path received: {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    Console.Error.WriteLine($"❌ File not found: {filePath}");
                    Log.Error("File not found: {FilePath}", filePath);
                    return 1;
                }

                string commitMessage;
                try
                {
                    commitMessage = File.ReadAllText(filePath)?.Trim() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"❌ Error reading file: {ex.Message}");
                    Log.Error(ex, "Error reading commit message file: {FilePath}", filePath);
                    return 1;
                }

                var validator = new CommitMessageValidator();
                if (validator.IsValid(commitMessage))
                {
                    Log.Information("Commit message validation succeeded.");
                    exitCode = 0;
                }
                else
                {
                    Log.Warning("Commit message validation failed.");
                    Console.Error.WriteLine("❌ Invalid commit message format.");
                    Console.Error.WriteLine($"   Message: '{commitMessage}'");
                    Console.Error.WriteLine("   Example: feat(parser): add ability to parse arrays");
                    string suggestion = validator.GetSuggestedMessage(commitMessage);
                    Console.Error.WriteLine($"🤖 Suggestion: {suggestion}");
                    Log.Information("Suggested commit message: {Suggestion}", suggestion);
                    exitCode = 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception in application.");
                exitCode = 1;
            }
            finally
            {
                Log.Information("Application exit with code {ExitCode}", exitCode);
                Log.CloseAndFlush();
            }
            return exitCode;
        }
    }
}
