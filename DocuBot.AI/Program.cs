
using System.Text.RegularExpressions;

namespace DocuBot.AI
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: DocuBot.AI <commit-message-file>");
                return 1;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"Error: File not found: {filePath}");
                return 1;
            }

            string commitMessage = File.ReadAllText(filePath).Trim();
            var validator = new CommitMessageValidator();
            if (validator.IsValid(commitMessage))
            {
                return 0;
            }
            else
            {
                Console.Error.WriteLine("\u001b[31mCommit message does not follow Conventional Commits format.\u001b[0m");
                Console.Error.WriteLine($"Message: '{commitMessage}'\n");
                Console.Error.WriteLine("Example format: 'feat(parser): add ability to parse arrays'\n");
                string suggestion = validator.Suggest(commitMessage);
                Console.Error.WriteLine($"Suggested commit message: {suggestion}");
                return 1;
            }
        }
    }

    public class CommitMessageValidator
    {
        // Conventional Commits: type(scope?): subject
        private static readonly Regex ConventionalRegex = new(
            @"^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\([\w\-\.]+\))?: .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool IsValid(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            var firstLine = message.Split('\n')[0].Trim();
            return ConventionalRegex.IsMatch(firstLine);
        }

        public string Suggest(string message)
        {
            // Try to extract a subject, fallback to a generic suggestion
            string subject = message.Split('\n')[0].Trim();
            if (string.IsNullOrWhiteSpace(subject))
                subject = "describe your change";
            return $"feat: {subject.ToLowerInvariant()}";
        }
    }
}
