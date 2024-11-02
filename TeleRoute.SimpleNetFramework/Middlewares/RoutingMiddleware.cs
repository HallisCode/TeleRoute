using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleNetFramework.Core.Middleware;
using Telegram.Bot.Types;
using TelegramWebApplication.Core.Routing;

namespace TelegramWebApplication.Infrastructure.Routing.Middlewares
{
    /// <summary>
    /// Вызывает контроллер для обработки текущего update. Не вызыает последующий обработчик middleware.
    /// </summary>
    public class RoutingMiddleware : IMiddleware<Update>
    {
        private readonly IRouteHandler _routeHandler;


        public MiddlewareDelegate<Update> Next { get; }


        public RoutingMiddleware(IRouteHandler routeHandler)
        {
            _routeHandler = routeHandler;
        }

        public async Task Invoke(Update update)
        {
            _routeHandler.Handle(update);
        }
    }
}