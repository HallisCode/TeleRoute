using System;

namespace TeleRoute.Infrastructure.Routing.Attributes
{
    /// <summary>
    /// Маркер, что данный класс/метод участвуют в маршрутизации
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TeleRouteAttribute : Attribute
    {
    }
}