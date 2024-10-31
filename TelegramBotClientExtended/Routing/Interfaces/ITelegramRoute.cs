using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public interface ITelegramRoute
    {
        UpdateType? AllowedTypes { get; }

        ITelegramFilter? Filter { get; }
    }
}