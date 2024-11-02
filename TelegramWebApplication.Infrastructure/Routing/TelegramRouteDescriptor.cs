using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public class TelegramRouteDescriptor : ITelegramRouteDescriptor
    {
        public UpdateType? AllowedType { get; protected set; }
        public ITelegramFilter[]? Filters { get; protected set; }

        public Type[]? NeededTypesForController { get; protected set; }
        public Type? ControllerType { get; protected set; }
        public MethodInfo? Handler { get; protected set; }
        public bool isBranch { get; protected set; }
        public ITelegramRouteDescriptor[]? InnerBranch { get; protected set; }

        protected TelegramRouteDescriptor()
        {
        }

        // Добавляет конечный обработчик
        public static TelegramRouteDescriptor CreateEndpoint(
            Type controllerType,
            MethodInfo handler,
            Type[] neededTypesForController,
            UpdateType? allowedType = null,
            ITelegramFilter[]? filters = null
        )
        {
            TelegramRouteDescriptor routeDescriptor = new TelegramRouteDescriptor();

            routeDescriptor.NeededTypesForController = neededTypesForController;
            routeDescriptor.ControllerType = controllerType;
            routeDescriptor.AllowedType = allowedType;
            routeDescriptor.Filters = filters;
            routeDescriptor.Handler = handler;

            return routeDescriptor;
        }

        // Добавляет ветвь
        public static TelegramRouteDescriptor CreateBranch(
            IEnumerable<ITelegramRouteDescriptor> innerBranch,
            UpdateType? allowedType = null,
            ITelegramFilter[]? filters = null
        )
        {
            TelegramRouteDescriptor routeDescriptor = new TelegramRouteDescriptor();

            routeDescriptor.AllowedType = allowedType;
            routeDescriptor.Filters = filters;
            routeDescriptor.InnerBranch = innerBranch.ToArray();
            routeDescriptor.isBranch = true;

            return routeDescriptor;
        }

        public virtual bool Equals(ITelegramRouteDescriptor? other)
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