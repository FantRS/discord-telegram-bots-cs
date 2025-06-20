using Claudia;

namespace MainSpace
{
    public abstract class PromptGenerator
    {
        protected readonly Anthropic _anthropic;

        protected PromptGenerator(Anthropic anthropic)
        {
            _anthropic = anthropic;
        }
    }
}
