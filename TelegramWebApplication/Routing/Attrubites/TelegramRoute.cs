using System;

namespace TelegramWebApplication.Routing.Attributes
{
    /// <summary>
    /// Маркер, что данный класс/метод участвуют в маршрутизации
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TelegramRouteAttribute : Attribute
    {
    }
}