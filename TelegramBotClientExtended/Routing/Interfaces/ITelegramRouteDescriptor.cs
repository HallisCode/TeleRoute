using System;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public interface ITelegramRouteDescriptor : IEquatable<ITelegramRouteDescriptor>
    {
        UpdateType? AllowedType { get; }
        
        ITelegramFilter[]? Filters { get; }

        TelegramEndpointDelegate? Handler { get; }

        bool isBranch { get; }

        ITelegramRouteDescriptor[]? InnerBranch { get; }
    }
}