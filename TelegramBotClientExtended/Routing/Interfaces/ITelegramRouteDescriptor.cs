using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TelegramBotClientExtended.Routing
{
    public interface ITelegramRouteDescriptor : ITelegramRoute, IEquatable<ITelegramRouteDescriptor>
    {
        TelegramEndpointDelegate? Handler { get; }

        bool isBranch { get; }

        ITelegramRouteDescriptor[]? InnerBranch { get; }
    }
}