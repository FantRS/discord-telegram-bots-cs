using Claudia;

namespace MainSpace
{
    public class SwearPromptGenerator : PromptGenerator
    {
        public SwearPromptGenerator(Anthropic anthropic) : base(anthropic) { }

        public async Task<string> GenerateAsync(string userMessage, string username, string? referencedMessage = null)
        {
            var messages = new List<Message>();

            if (referencedMessage != null)
            {
                messages.Add(new() { Role = "assistant", Content = referencedMessage });
            }

            messages.Add(new() { Role = "user", Content = userMessage });

            var message = await _anthropic.Messages.CreateAsync(new MessageRequest()
            {
                Model = "claude-3-5-sonnet-20240620",
                MaxTokens = 1024,
                Messages = messages.ToArray(),
                System = Configuration.GetPrompt(username)
            });

            return message.Content.ToString();
        }
    }
}
