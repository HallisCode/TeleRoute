using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing;

namespace TeleRoute.Infrastructure.Routing
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

            // Сохраняем количество пройденных фильтров дескриптором, чтобы найти наилучшее совпадение
            int maxPassedFiltersCount = 0;

            foreach (IRouteDescriptor descriptor in descriptors)
            {
                // Проверяем какие условия заданы
                bool isAllowedTypeDefined = descriptor.AllowedType is not UpdateType.Unknown;
                bool isFilterDefined = descriptor.Filters.Length > 0;

                // Определяем соответствует ли update заданному типу
                // Если type is 'Unknown' -> разрешены все типы
                bool isTypePassed = !isAllowedTypeDefined || descriptor.AllowedType.Equals(updateType);
                if (!isTypePassed)
                {
                    continue;
                }

                // Определяем соответствует ли update фильтрам, если нет, пропускаем этот дескриптор
                int countPassedFilters = 0;
                if (isFilterDefined)
                {
                    countPassedFilters = descriptor.Filters.Count(filter =>
                        filter.IsMatch(update) && filter.IsTypeAllowed(updateType)
                    );
                }

                // Если присутствуют вложенные маршруты, по ним проходимся тоже
                if (descriptor.isBranch)
                {
                    return _Resolve(update, descriptor.InnerBranch!);
                }

                if (isTypePassed && countPassedFilters > maxPassedFiltersCount)
                {
                    _descriptor = descriptor;
                }
            }

            return _descriptor;
        }
    }
}