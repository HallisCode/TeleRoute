using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Core.Routing
{
    public interface IRouteTree
    {
        IReadOnlyCollection<IRouteDescriptor> Routings { get; }

        IRouteDescriptor? Resolve(Update update);
    }
}