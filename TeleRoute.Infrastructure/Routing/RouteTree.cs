using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing;
using TeleRoute.Core.Routing.Filters;

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
                IRouteDescriptor? processedDescriptor = descriptor;

                // Проверяем какие условия заданы
                bool isAllowedTypeDefined = processedDescriptor.AllowedType is not UpdateType.Unknown;
                bool isFilterDefined = processedDescriptor.Filters.Length > 0;

                // Определяем соответствует ли update заданному типу
                // Если type is 'Unknown' -> разрешены все типы
                bool isTypePassed = !isAllowedTypeDefined || processedDescriptor.AllowedType.Equals(updateType);
                if (!isTypePassed)
                {
                    continue;
                }
                
                // Считаем, сколько фильтров endpoint'a прошёл update.
                int countPassedFilters = 0;
                if (isFilterDefined)
                {
                    foreach (IFilter filter in processedDescriptor.Filters)
                    {
                        bool isFilterPassed = filter.IsMatch(update);
                        bool isFilterTypePassed = filter.IsTypeAllowed(updateType);

                        if (isFilterPassed && isFilterTypePassed)
                        {
                            countPassedFilters++;
                        }
                    }
                }
                
                // Определяем, прошёл ли update все фильтры endpoint'a.
                bool isFiltersPassed = true;
                isFiltersPassed = processedDescriptor.Filters.Length == countPassedFilters;

                // Если присутствуют вложенные маршруты, по ним проходимся тоже,
                // тем самым получим вложенный маршрут с наилучшим совпадением.
                if (processedDescriptor.isBranch)
                {
                    processedDescriptor = _Resolve(update, processedDescriptor.InnerBranch!);
                }

                // Если 2 маршрута с одинаковым количеством пройденных фильтров, берётся последний
                if (isTypePassed && isFiltersPassed && countPassedFilters >= maxPassedFiltersCount)
                {
                    _descriptor = processedDescriptor;

                    maxPassedFiltersCount = countPassedFilters;
                }
            }

            return _descriptor;
        }
    }
}