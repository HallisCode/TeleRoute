using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TeleRoute.Core.Routing
{
    public interface IRouteTree
    {
        IReadOnlyCollection<IRouteDescriptor> Routings { get; }

        IRouteDescriptor? Resolve(Update update);
    }
}