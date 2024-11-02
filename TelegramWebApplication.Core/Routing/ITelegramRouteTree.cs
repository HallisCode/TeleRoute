using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Core.Routing
{
    public interface ITelegramRouteTree
    {
        IReadOnlyCollection<ITelegramRouteDescriptor> Routings { get; }

        ITelegramRouteDescriptor? Resolve(Update update);
    }
}