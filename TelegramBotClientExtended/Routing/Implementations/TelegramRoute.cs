using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public record TelegramRoute : ITelegramRoute
    {
        public UpdateType? AllowedType { get; init; }
        public ITelegramFilter? Filter { get; init; }

        public TelegramRoute(UpdateType allowedTypes, ITelegramFilter? filter = null)
        {
            AllowedType = allowedTypes;
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