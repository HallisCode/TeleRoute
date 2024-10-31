using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotClientExtended.Routing.Filters
{
    public interface ITelegramFilter
    {
        UpdateType AllowedTypes { get; }

        bool isMatch(Update update);


        bool IsTypeConformsAllowedType(UpdateType type);
    }
}