using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public interface ITelegramRoute
    {
        UpdateType? AllowedType { get; }

        ITelegramFilter? Filter { get; }
    }
}