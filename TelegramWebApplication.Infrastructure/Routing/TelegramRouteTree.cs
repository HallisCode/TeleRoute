using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing;
using TelegramWebApplication.Core.Routing.Filters;

namespace TelegramWebApplication.Infrastructure.Routing
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
            ITelegramRouteDescriptor? _descriptor = null;

            TakeDescriptorLoop:
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

                // Определяем соответствует ли update фильтрам, если нет, пропускаем этот дескриптор
                if (isFilterDefined)
                {
                    foreach (ITelegramFilter filter in routeDescriptor.Filters)
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
                    _descriptor = _Resolve(update, routeDescriptor.InnerBranch!);
                }

                // Если были определены оба условия и они пройдены, то это более точное совпадение, нету смысла
                // дальше искать маршрут. Иначе продолжаем искать до более точного совпадения
                if (isAllowedTypeDefined && isFilterDefined)
                {
                    return _descriptor;
                }
            }

            return _descriptor;
        }
    }
}