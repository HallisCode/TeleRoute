using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public class RouteDescriptor : IRouteDescriptor
    {
        public UpdateType AllowedType { get; protected set; }
        public IFilter[]? Filters { get; protected set; }

        public Type[]? NeededTypesForController { get; protected set; }
        public Type? ControllerType { get; protected set; }
        public MethodInfo? Handler { get; protected set; }
        public bool isBranch { get; protected set; }
        public IRouteDescriptor[]? InnerBranch { get; protected set; }

        protected RouteDescriptor()
        {
        }

        // Добавляет конечный обработчик
        public static RouteDescriptor CreateEndpoint(
            Type controllerType,
            MethodInfo handler,
            Type[] neededTypesForController,
            UpdateType allowedType = UpdateType.Unknown,
            IFilter[]? filters = null
        )
        {
            RouteDescriptor routeDescriptor = new RouteDescriptor();

            routeDescriptor.NeededTypesForController = neededTypesForController;
            routeDescriptor.ControllerType = controllerType;
            routeDescriptor.AllowedType = allowedType;
            routeDescriptor.Filters = filters;
            routeDescriptor.Handler = handler;

            return routeDescriptor;
        }

        // Добавляет ветвь
        public static RouteDescriptor CreateBranch(
            IEnumerable<IRouteDescriptor> innerBranch,
            UpdateType allowedType = UpdateType.Unknown,
            IFilter[]? filters = null
        )
        {
            RouteDescriptor routeDescriptor = new RouteDescriptor();

            routeDescriptor.AllowedType = allowedType;
            routeDescriptor.Filters = filters;
            routeDescriptor.InnerBranch = innerBranch.ToArray();
            routeDescriptor.isBranch = true;

            return routeDescriptor;
        }

        public virtual bool Equals(IRouteDescriptor? other)
        {
            if (other is not null)
            {
                return this.GetHashCode() == other.GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AllowedType, Filters);
        }
    }
}