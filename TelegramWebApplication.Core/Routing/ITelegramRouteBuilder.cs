using System;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Infrastructure.Routing.Filters;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public interface ITelegramRouteBuilder
    {
        /// <summary>
        /// Добавляет все маршруты из сборки, на основа аттрибутов типа <see cref="TelegramRoute"/>.
        /// </summary>
        /// <param name="assembly">Сборка из которой нужно найти все маршруты.</param>
        /// <returns></returns>
        ITelegramRouteBuilder AddFromAssembly(Assembly assembly);

        /// <summary>
        /// Добавляет маршрут.
        /// </summary>
        /// <param name="routeDescriptor"></param>
        /// <returns></returns>
        ITelegramRouteBuilder Add(ITelegramRouteDescriptor routeDescriptor);

        /// <summary>
        /// Создаёт древо маршрутов.
        /// </summary>
        /// <returns></returns>
        ITelegramRouteTree Build();
    }
}