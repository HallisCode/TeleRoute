using System;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TelegramRouteAttribute : Attribute, ITelegramRoute
    {
        public UpdateType? AllowedType { get; private set; } = null;
        public ITelegramFilter? Filter { get; private set; }
        

        public TelegramRouteAttribute(UpdateType type, ITelegramFilter? filter = null)
        {
            AllowedType = type;
            Filter = filter;
        }

        public TelegramRouteAttribute(ITelegramFilter filter)
        {
            Filter = filter;
        }
    }
}