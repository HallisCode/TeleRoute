using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public record TelegramRouteDescriptor : ITelegramRouteDescriptor
    {
        public UpdateType? AllowedType { get; init; }
        public ITelegramFilter? Filter { get; init; }
        public TelegramEndpointDelegate? Handler { get; init; }

        public bool isBranch { get; }

        public ITelegramRouteDescriptor[]? InnerBranch { get; init; }

        public TelegramRouteDescriptor(
            TelegramEndpointDelegate handler,
            UpdateType? allowedTypes = null,
            ITelegramFilter? filter = null)
        {
            AllowedType = allowedTypes;
            Filter = filter;
            Handler = handler;
        }

        public TelegramRouteDescriptor(
            IEnumerable<ITelegramRouteDescriptor> innerBranch,
            UpdateType? allowedTypes = null,
            ITelegramFilter? filter = null)
        {
            AllowedType = allowedTypes;
            Filter = filter;
            InnerBranch = innerBranch.ToArray();
            isBranch = true;
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
            return HashCode.Combine(AllowedType, Filter);
        }
    }
}