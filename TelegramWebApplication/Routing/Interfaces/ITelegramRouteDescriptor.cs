using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Routing.Filters;

namespace TelegramWebApplication.Routing
{
    public interface ITelegramRouteDescriptor : IEquatable<ITelegramRouteDescriptor>
    {
        UpdateType? AllowedType { get; }
        ITelegramFilter[]? Filters { get; }

        Type? ControllerType { get; }
        MethodInfo? Handler { get; }
        bool isBranch { get; }
        ITelegramRouteDescriptor[]? InnerBranch { get; }
    }
}