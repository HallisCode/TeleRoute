using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;

namespace TeleRoute.Core.Routing
{
    /// <summary>
    /// Описывает конечную точку маршрута.
    /// </summary>
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