using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public record TelegramRoute : ITelegramRoute
    {
        public UpdateType? AllowedTypes { get; init; }
        public ITelegramFilter? Filter { get; init; }

        public TelegramRoute(UpdateType allowedTypes, ITelegramFilter? filter = null)
        {
            AllowedTypes = allowedTypes;
            Filter = filter;
        }
        
        public TelegramRoute(ITelegramFilter filter)
        {
            Filter = filter;
        }
        
        public TelegramRoute()
        {
        }
    }
}