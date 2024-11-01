using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Filters;

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
                bool isFilterDefined = routeDescriptor.Filters is not null;

                // Определяем соответствует ли update фильтрам
                if (isFilterDefined)
                {
                    foreach (ITelegramFilter filter in routeDescriptor.Filters)
                    {
                        bool isFilterPassed = filter.IsMatch(update);

                        if (!isFilterPassed) continue;
                    }
                }

                // Определяем соответствует ли update заданному типу
                bool isTypePassed = isAllowedTypeDefined && routeDescriptor.AllowedType.Equals(updateType);
                if (!isTypePassed)
                {
                    continue;
                }

                return routeDescriptor;
            }

            return _routeDescriptor;
        }
    }
}