using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotClientExtended.Routing.Filters
{
    public interface ITelegramFilter
    {
        UpdateType? AllowedType { get; }
        bool IsMatch(Update update);
        bool IsTypeConformsAllowedType(UpdateType type);
    }
}