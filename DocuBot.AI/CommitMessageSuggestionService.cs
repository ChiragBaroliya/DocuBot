namespace DocuBot.AI
{
    public interface ICommitMessageSuggestionService
    {
        string GenerateSuggestedMessage(string originalMessage, string gitDiffSummary);
    }

    public class CommitMessageSuggestionService : ICommitMessageSuggestionService
    {
        public virtual string GenerateSuggestedMessage(string originalMessage, string gitDiffSummary)
        {
            // Mock AI response for now
            return "feat(core): improve login error handling";
        }
    }
}
