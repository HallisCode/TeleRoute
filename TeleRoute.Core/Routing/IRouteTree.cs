using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TeleRoute.Core.Routing
{
    /// <summary>
    /// Описывает древо маршрутов.
    /// </summary>
    public interface IRouteTree
    {
        IReadOnlyCollection<IRouteDescriptor> Routings { get; }

        IRouteDescriptor? Resolve(Update update);
    }
}