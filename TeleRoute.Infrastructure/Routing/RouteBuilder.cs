using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TeleRoute.Core.Routing;
using TeleRoute.Core.Routing.Filters;
using TeleRoute.Infrastructure.Routing.Attributes;

namespace TeleRoute.Infrastructure.Routing
{
    public class RouteBuilder : IRouteBuilder
    {
        private List<IRouteDescriptor> _routes = new List<IRouteDescriptor>();

        public RouteBuilder()
        {
        }

        public IRouteBuilder AddFromAssembly(Assembly assembly)
        {
            _AddAllClassesWithTelegramRouteAttributeFromAssembly(assembly);

            return this;
        }

        public IRouteBuilder Add(IRouteDescriptor routeDescriptor)
        {
            _routes.Add(routeDescriptor);

            return this;
        }

        public IRouteTree Build()
        {
            _VerifyUnduplicated(_routes);
            IRouteTree routeTree = new RouteTree(_routes);

            _routes.Clear();
            return routeTree;
        }

        private void _AddAllClassesWithTelegramRouteAttributeFromAssembly(Assembly assembly)
        {
            // Находим все классы с TeleRouteAttribute
            Type[] types = assembly.GetTypes();
            Type[] classTypeWithAttribute = types.Where(
                (type) => type.IsClass && Attribute.IsDefined(type, typeof(TeleRouteAttribute))
            ).ToArray();

            List<IRouteDescriptor> descriptors = new List<IRouteDescriptor>();
            foreach (Type classType in classTypeWithAttribute)
            {
                object[] attributes = classType.GetCustomAttributes(false);

                AllowedUpdateTypeAttribute? allowedTypeAttribute = (AllowedUpdateTypeAttribute?)
                    Attribute.GetCustomAttribute(
                        classType, typeof(AllowedUpdateTypeAttribute)
                    );
                IFilter[] filtersAttributes = attributes.OfType<IFilter>().ToArray();

                // Получаем все методы класса с аттрибутом TeleRouteAttribute
                MethodInfo[] methodsWithAttribute = classType.GetMethods().Where(
                    (method) => Attribute.IsDefined(method, typeof(TeleRouteAttribute))
                ).ToArray();
                if (methodsWithAttribute.Length <= 0)
                {
                    continue;
                }

                // Получаем дескрипторы для всех методов, по аналогии с классом
                List<IRouteDescriptor> methodsDescriptors = _GetRoutesFromMethods(methodsWithAttribute);

                // Проверяем какие условия заданы у класса
                bool isAllowedTypeDefined = allowedTypeAttribute is not null;
                bool isFilterDefined = filtersAttributes.Length > 0;
                
                // Если у класса-контроллера присутсвуют ограничения => создаём вложенную ветку с маршрутами,
                // иначе добавляем все endpoints.
                bool isClassHasConditions = isAllowedTypeDefined || isFilterDefined;
                if (isClassHasConditions)
                {
                    IRouteDescriptor descriptor = RouteDescriptor.CreateBranch(
                        innerBranch: methodsDescriptors,
                        allowedType: allowedTypeAttribute?.AllowedType ?? UpdateType.Unknown,
                        filters: filtersAttributes
                    );

                    descriptors.Add(descriptor);
                }
                else
                {
                    descriptors.AddRange(methodsDescriptors);
                }
            }

            _VerifyUnduplicated(descriptors);
            _routes.AddRange(descriptors);
        }

        /// <summary>
        /// Получает endpoints на основе переданных методов.
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<IRouteDescriptor> _GetRoutesFromMethods(IEnumerable<MethodInfo> methods)
        {
            List<IRouteDescriptor> descriptors = new List<IRouteDescriptor>();

            foreach (MethodInfo method in methods)
            {
                // Проверяем сигнатуру метода
                _VerifyMatchTelegramEndpointDelegate(method);

                TeleRouteAttribute? methodRouteAttribute = (TeleRouteAttribute?)
                    Attribute.GetCustomAttribute(method, typeof(TeleRouteAttribute));

                // Проверяем что у метода присутствует аттрибут TeleRouteAttribute
                if (methodRouteAttribute is null)
                {
                    throw new Exception($"У метода {method.Module.Assembly.Location} отсутствует аттрибут " +
                                        $"{typeof(TeleRouteAttribute).FullName}");
                }

                object[] attributes = method.GetCustomAttributes(false);

                // Получаем аттрибут AllowedUpdateType навешанный на метод
                AllowedUpdateTypeAttribute? allowedTypeAttribute = (AllowedUpdateTypeAttribute?)
                    Attribute.GetCustomAttribute(
                        method, typeof(AllowedUpdateTypeAttribute)
                    );
                
                // Получаем фильтры навешанные на метод
                IFilter[] filtersAttributes = attributes.OfType<IFilter>().ToArray();

                // Получаем массив необходимых зависимостей для контроллера
                Type[] neededTypesForController = method.DeclaringType!
                    .GetConstructors().First()
                    .GetParameters()
                    .Select((ParameterInfo parameter) => parameter.ParameterType).ToArray();
                
                // Создаём ednpoint
                RouteDescriptor routeDescriptor = RouteDescriptor.CreateEndpoint(
                    controllerType: method.DeclaringType,
                    handler: method,
                    allowedType: allowedTypeAttribute?.AllowedType ?? UpdateType.Unknown,
                    filters: filtersAttributes,
                    neededTypesForController: neededTypesForController);

                descriptors.Add(routeDescriptor);
            }
            
            _VerifyUnduplicated(descriptors);
            return descriptors;
        }

        /// <summary>
        /// Проверяет, чтобы метод соответствовал сигнатуре <see cref="EndpointDelegate"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <exception cref="Exception"></exception>
        private void _VerifyMatchTelegramEndpointDelegate(MethodInfo method)
        {
            Type telegramEndpointDelegate = typeof(EndpointDelegate);
            MethodInfo telegramEndpointMethod = telegramEndpointDelegate.GetMethod("Invoke")!;

            ParameterInfo[] telegramEndpointParams = method.GetParameters();
            ParameterInfo[] methodParams = method.GetParameters();

            // Проверяем возвращаемый тип
            if (method.ReturnType != telegramEndpointMethod.ReturnType)
            {
                goto ThrowException;
            }

            // Проверяем количество параметров
            if (telegramEndpointParams.Length != methodParams.Length)
            {
                goto ThrowException;
            }

            foreach (ParameterInfo parameter in telegramEndpointParams)
            {
                if (methodParams.Contains(parameter) is false)
                {
                    goto ThrowException;
                }
            }

            return;

            ThrowException:
            throw new Exception(
                $"Сигнатура метода {method} не соввпадает с {typeof(EndpointDelegate).FullName}"
            );
        }

        /// <summary>
        /// Проверяет на присутствие равноценных маршрутов.
        /// </summary>
        /// <param name="descriptors"></param>
        /// <exception cref="Exception"></exception>
        private void _VerifyUnduplicated(IEnumerable<IRouteDescriptor> descriptors)
        {
            if (descriptors.Count() != new HashSet<IRouteDescriptor>(descriptors).Count)
            {
                throw new Exception($"Были найдены дублирующиеся routes. " +
                                    $"Даже наличие двух методов с пустыми аттрибутами {typeof(TeleRouteAttribute)} " +
                                    $"является ошибкой, так как маршрутизация должна быть точной." +
                                    $"\nОшибка не не содержит полезной информации ? " +
                                    $"t.me/HallisPlus напишите разработчику что он слишком ленивый, пусть исправляет.");
            }
        }
    }
}