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
            // Находим все классы с TelegramRouteAttribute
            Type[] types = assembly.GetTypes();
            Type[] classTypeWithAttribute = types.Where(
                (type) => type.IsClass && Attribute.IsDefined(type, typeof(TelegramRouteAttribute))
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

                // Получаем все методы класса с аттрибутом TelegramRouteAttribute
                MethodInfo[] methodsWithAttribute = classType.GetMethods().Where(
                    (method) => Attribute.IsDefined(method, typeof(TelegramRouteAttribute))
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

                bool isClassHasConditions = isAllowedTypeDefined || isFilterDefined;
                if (isClassHasConditions)
                {
                    IRouteDescriptor descriptor = RouteDescriptor.CreateBranch(
                        innerBranch: methodsDescriptors,
                        allowedType: allowedTypeAttribute?.AllowedType ?? UpdateType.Unknown,
                        filters: filtersAttributes
                    );

                    descriptors.Add(descriptor);
                    continue;
                }

                descriptors.AddRange(methodsDescriptors);
            }

            _VerifyUnduplicated(descriptors);
            _routes.AddRange(descriptors);
        }

        private List<IRouteDescriptor> _GetRoutesFromMethods(IEnumerable<MethodInfo> methods)
        {
            List<IRouteDescriptor> descriptors = new List<IRouteDescriptor>();

            foreach (MethodInfo method in methods)
            {
                _VerifyMatchTelegramEndpointDelegate(method);

                TelegramRouteAttribute? methodRouteAttribute = (TelegramRouteAttribute?)
                    Attribute.GetCustomAttribute(method, typeof(TelegramRouteAttribute));

                // Проверяем что у метода присутствует аттрибут TelegramRouteAttribute
                if (methodRouteAttribute is null)
                {
                    throw new Exception($"У метода {method.Module.Assembly.Location} отсутствует аттрибут " +
                                        $"{typeof(TelegramRouteAttribute).FullName}");
                }

                object[] attributes = method.GetCustomAttributes(false);

                AllowedUpdateTypeAttribute? allowedTypeAttribute = (AllowedUpdateTypeAttribute?)
                    Attribute.GetCustomAttribute(
                        method, typeof(AllowedUpdateTypeAttribute)
                    );
                IFilter[] filtersAttributes = attributes.OfType<IFilter>().ToArray();


                Type[] neededTypesForController = method.DeclaringType
                    .GetConstructors().First()
                    .GetParameters()
                    .Select((ParameterInfo parameter) => parameter.ParameterType).ToArray();

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

        private void _VerifyMatchTelegramEndpointDelegate(MethodInfo method)
        {
            Type telegramEndpointDelegate = typeof(EndpointDelegate);
            MethodInfo telegramEndpointMethod = telegramEndpointDelegate.GetMethod("Invoke")!;

            ParameterInfo[] telegramEndpointParams = method.GetParameters();
            ParameterInfo[] methodParams = method.GetParameters();

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

        private void _VerifyUnduplicated(IEnumerable<IRouteDescriptor> descriptors)
        {
            if (descriptors.Count() != new HashSet<IRouteDescriptor>(descriptors).Count)
            {
                throw new Exception($"Были найдены дублирующиеся routes. " +
                                    $"Даже наличие двух методов с пустыми аттрибутами {typeof(TelegramRouteAttribute)} " +
                                    $"является ошибкой, так как маршрутизация должна быть точной." +
                                    $"\nОшибка не не содержит полезной информации ? " +
                                    $"t.me/HallisPlus напишите разработчику что он слишком ленивый, пусть исправляет.");
            }
        }
    }
}