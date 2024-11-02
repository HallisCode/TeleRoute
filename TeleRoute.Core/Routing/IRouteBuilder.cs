using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing.Filters;

namespace TeleRoute.Core.Routing
{
    public interface IRouteBuilder
    {
        /// <summary>
        /// Добавляет все маршруты из сборки, на основа аттрибутов типа <see cref="TelegramRoute"/>.
        /// </summary>
        /// <param name="assembly">Сборка из которой нужно найти все маршруты.</param>
        /// <returns></returns>
        IRouteBuilder AddFromAssembly(Assembly assembly);

        /// <summary>
        /// Добавляет маршрут.
        /// </summary>
        /// <param name="routeDescriptor"></param>
        /// <returns></returns>
        IRouteBuilder Add(IRouteDescriptor routeDescriptor);

        /// <summary>
        /// Создаёт древо маршрутов.
        /// </summary>
        /// <returns></returns>
        IRouteTree Build();
    }
}