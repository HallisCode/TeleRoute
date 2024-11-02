using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public class RouteTree : IRouteTree
    {
        public IReadOnlyCollection<IRouteDescriptor> Routings { get; private set; }


        public RouteTree(IEnumerable<IRouteDescriptor> routings)
        {
            Routings = routings.ToArray();
        }

        public IRouteDescriptor? Resolve(Update update)
        {
            return _Resolve(update, Routings);
        }


        // Ищем конечный дескриптор на основе прохода древа
        private IRouteDescriptor? _Resolve(
            Update update,
            IEnumerable<IRouteDescriptor> descriptors
        )
        {
            UpdateType updateType = update.Type;
            IRouteDescriptor? _descriptor = null;

            TakeDescriptorLoop:
            foreach (IRouteDescriptor routeDescriptor in descriptors)
            {
                // Проверяем какие условия заданы
                bool isAllowedTypeDefined = routeDescriptor.AllowedType is not UpdateType.Unknown;
                bool isFilterDefined = routeDescriptor.Filters.Length > 0;

                // Определяем соответствует ли update заданному типу
                // Если type is 'Unknown' -> разрешены все типы
                bool isTypePassed = !isAllowedTypeDefined || routeDescriptor.AllowedType.Equals(updateType);
                if (!isTypePassed)
                {
                    continue;
                }

                // Определяем соответствует ли update фильтрам, если нет, пропускаем этот дескриптор
                if (isFilterDefined)
                {
                    foreach (IFilter filter in routeDescriptor.Filters)
                    {
                        if (!filter.IsMatch(update))
                        {
                            goto TakeDescriptorLoop;
                        }
                    }
                }

                // Если присутствуют вложенные маршруты, по ним проходимся тоже
                if (routeDescriptor.isBranch)
                {
                    return _Resolve(update, routeDescriptor.InnerBranch!);
                }

                // Если были определены оба условия и они пройдены, то это более точное совпадение, нету смысла
                // дальше искать маршрут. Иначе продолжаем искать до более точного совпадения
                if (isAllowedTypeDefined && isFilterDefined)
                {
                    return _descriptor;
                }

                _descriptor = routeDescriptor;
            }

            return _descriptor;
        }
    }
}