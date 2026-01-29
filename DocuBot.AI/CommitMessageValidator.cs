using System;
using System.Text.RegularExpressions;

namespace DocuBot.AI
{
    public class CommitMessageValidator
    {
        // Allowed types: feat, fix, docs, refactor, test, chore
        protected virtual Regex ConventionalRegex { get; } = new(
            @"^(feat|fix|docs|refactor|test|chore)(\([\w\-\.]+\))?: .{10,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool IsValid(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            var firstLine = message.Split('\n')[0].Trim();
            return ConventionalRegex.IsMatch(firstLine);
        }

        public virtual string GetSuggestedMessage(string originalMessage)
        {
            if (string.IsNullOrWhiteSpace(originalMessage))
                return "fix(core): describe your change";

            string trimmed = originalMessage.Trim();
            // Try to extract a type at the start
            var match = Regex.Match(trimmed, @"^(feat|fix|docs|refactor|test|chore)\b", RegexOptions.IgnoreCase);
            string type = match.Success ? match.Groups[1].Value.ToLowerInvariant() : "fix";

            // Remove the type if present
            string rest = match.Success ? trimmed.Substring(match.Length).TrimStart() : trimmed;

            // Normalize casing: first letter lowercase, rest as-is
            if (!string.IsNullOrEmpty(rest))
            {
                rest = char.ToLowerInvariant(rest[0]) + (rest.Length > 1 ? rest.Substring(1) : "");
            }
            else
            {
                rest = "describe your change";
            }

            // Always use (core) as the default scope
            return $"{type}(core): {rest}";
        }
    }
}
