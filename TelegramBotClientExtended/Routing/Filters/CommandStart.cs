using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotClientExtended.Routing.Filters
{
    public class CommandStart : ITelegramFilter
    {
        public UpdateType AllowedTypes { get; }

        public bool isMatch(Update update)
        {
            throw new System.NotImplementedException();
        }

        public bool IsTypeConformsAllowedType(UpdateType type)
        {
            
        }
    }
}