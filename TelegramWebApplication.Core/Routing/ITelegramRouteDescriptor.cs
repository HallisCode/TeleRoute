using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Core.Routing
{
    public interface ITelegramRouteDescriptor : IEquatable<ITelegramRouteDescriptor>
    {
        UpdateType AllowedType { get; }
        ITelegramFilter[]? Filters { get; }

        Type[]? NeededTypesForController { get; }
        Type? ControllerType { get; }
        MethodInfo? Handler { get; }
        bool isBranch { get; }
        ITelegramRouteDescriptor[]? InnerBranch { get; }
    }
}