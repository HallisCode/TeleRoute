using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotClientExtended.Routing
{
    public class TelegramRouteTree : ITelegramRouteTree
    {
        public IReadOnlyCollection<ITelegramRouteDescriptor> Routings { get; private set; }


        public TelegramRouteTree(IEnumerable<ITelegramRouteDescriptor> routings)
        {
            Routings = routings.ToArray();
        }

        public ITelegramRouteDescriptor? Resolve(Update update)
        {
            UpdateType updateType = update.Type;


            ITelegramRouteDescriptor? _routeDescriptor = null;
            foreach (ITelegramRouteDescriptor routeDescriptor in Routings)
            {
                // Проверяем какие условия заданы
                bool isAllowedTypeDefined = routeDescriptor.AllowedType is not null;
                bool isFilterDefined = routeDescriptor.Filter is not null;

                // Определяем соответствует ли update типу фильтра при его наличии и проходит ли update фильтр
                bool isFilterTypeMatch = isFilterDefined && routeDescriptor.Filter!.AllowedTypes.Equals(updateType);
                bool isFilterMatch = isFilterTypeMatch && routeDescriptor.Filter!.isMatch(update);
                
                // Определяем соответствует ли update заданному типу
                bool isTypeMatch = isAllowedTypeDefined && routeDescriptor.AllowedType.Equals(updateType);

                /* P.S У фильтра есть собственные разрешенные типы, которые он допускает к обработке, поэтому
                 мы проверяем обрабатывает ли фильтр заданный тип у update
                 */
                if (isFilterMatch && isTypeMatch)
                {
                    return routeDescriptor;
                }

                if (isFilterMatch || isTypeMatch)
                {
                    _routeDescriptor = routeDescriptor;
                }
            }

            return _routeDescriptor;
        }
    }
}