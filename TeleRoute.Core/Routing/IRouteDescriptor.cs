using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Core.Routing
{
    public interface IRouteDescriptor : IEquatable<IRouteDescriptor>
    {
        UpdateType AllowedType { get; }
        IFilter[]? Filters { get; }

        Type[]? NeededTypesForController { get; }
        Type? ControllerType { get; }
        MethodInfo? Handler { get; }
        bool isBranch { get; }
        IRouteDescriptor[]? InnerBranch { get; }
    }
}