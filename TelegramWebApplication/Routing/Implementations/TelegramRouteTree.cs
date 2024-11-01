using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Routing.Filters;

namespace TelegramWebApplication.Routing
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
            return _Resolve(update, Routings);
        }

        // Ищем конечный дескриптор на основе прохода древа
        private ITelegramRouteDescriptor? _Resolve(
            Update update,
            IEnumerable<ITelegramRouteDescriptor> descriptors
        )
        {
            UpdateType updateType = update.Type;

            foreach (ITelegramRouteDescriptor routeDescriptor in descriptors)
            {
                // Проверяем какие условия заданы
                bool isAllowedTypeDefined = routeDescriptor.AllowedType is not null;
                bool isFilterDefined = routeDescriptor.Filters is not null;

                // Определяем соответствует ли update заданному типу
                bool isTypePassed = isAllowedTypeDefined && routeDescriptor.AllowedType.Equals(updateType);
                if (!isTypePassed)
                {
                    continue;
                }

                // Определяем соответствует ли update фильтрам
                if (isFilterDefined)
                {
                    foreach (ITelegramFilter filter in routeDescriptor.Filters)
                    {
                        bool isFilterPassed = filter.IsMatch(update);

                        if (!isFilterPassed) continue;
                    }
                }

                if (routeDescriptor.isBranch)
                {
                    return _Resolve(update, routeDescriptor.InnerBranch!);
                }

                return routeDescriptor;
            }

            return null;
        }
    }
}