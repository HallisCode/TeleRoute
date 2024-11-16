using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleRoute.Core.Routing
{
    public interface IRouteTree
    {
        IReadOnlyCollection<IRouteDescriptor> Routings { get; }

        Task<IRouteDescriptor>? Resolve(Update update);
    }
}