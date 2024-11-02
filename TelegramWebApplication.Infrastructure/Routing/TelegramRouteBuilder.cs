using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramWebApplication.Core.Routing;
using TelegramWebApplication.Core.Routing.Filters;
using TelegramWebApplication.Infrastructure.Routing.Attributes;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public class TelegramRouteBuilder : ITelegramRouteBuilder
    {
        private List<ITelegramRouteDescriptor> _routes = new List<ITelegramRouteDescriptor>();

        public TelegramRouteBuilder()
        {
        }

        public ITelegramRouteBuilder AddFromAssembly(Assembly assembly)
        {
            _AddAllClassesWithTelegramRouteAttributeFromAssembly(assembly);

            return this;
        }

        public ITelegramRouteBuilder Add(ITelegramRouteDescriptor routeDescriptor)
        {
            _routes.Add(routeDescriptor);

            return this;
        }

        public ITelegramRouteTree Build()
        {
            _VerifyUnduplicated(_routes);
            ITelegramRouteTree routeTree = new TelegramRouteTree(_routes);

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

            List<ITelegramRouteDescriptor> descriptors = new List<ITelegramRouteDescriptor>();
            foreach (Type classType in classTypeWithAttribute)
            {
                object[] attributes = classType.GetCustomAttributes(false);

                AllowedUpdateTypeAttribute? allowedTypeAttribute = (AllowedUpdateTypeAttribute?)
                    Attribute.GetCustomAttribute(
                        classType, typeof(AllowedUpdateTypeAttribute)
                    );
                ITelegramFilter[] filtersAttributes = attributes.OfType<ITelegramFilter>().ToArray();

                // Получаем все методы класса с аттрибутом TelegramRouteAttribute
                MethodInfo[] methodsWithAttribute = classType.GetMethods().Where(
                    (method) => Attribute.IsDefined(method, typeof(TelegramRouteAttribute))
                ).ToArray();
                if (methodsWithAttribute.Length <= 0)
                {
                    continue;
                }

                // Получаем дескрипторы для всех методов, по аналогии с классом
                List<ITelegramRouteDescriptor> methodsDescriptors = _GetRoutesFromMethods(methodsWithAttribute);

                // Проверяем какие условия заданы у класса
                bool isAllowedTypeDefined = allowedTypeAttribute is not null;
                bool isFilterDefined = filtersAttributes.Length > 0;

                bool isClassHasConditions = isAllowedTypeDefined || isFilterDefined;
                if (isClassHasConditions)
                {
                    ITelegramRouteDescriptor descriptor = TelegramRouteDescriptor.CreateBranch(
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

        private List<ITelegramRouteDescriptor> _GetRoutesFromMethods(IEnumerable<MethodInfo> methods)
        {
            List<ITelegramRouteDescriptor> descriptors = new List<ITelegramRouteDescriptor>();

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
                ITelegramFilter[] filtersAttributes = attributes.OfType<ITelegramFilter>().ToArray();


                Type[] neededTypesForController = method.DeclaringType
                    .GetConstructors().First()
                    .GetParameters()
                    .Select((ParameterInfo parameter) => parameter.ParameterType).ToArray();

                TelegramRouteDescriptor routeDescriptor = TelegramRouteDescriptor.CreateEndpoint(
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
            Type telegramEndpointDelegate = typeof(TelegramEndpointDelegate);
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
                $"Сигнатура метода {method} не соввпадает с {typeof(TelegramEndpointDelegate).FullName}"
            );
        }

        private void _VerifyUnduplicated(IEnumerable<ITelegramRouteDescriptor> descriptors)
        {
            if (descriptors.Count() != new HashSet<ITelegramRouteDescriptor>(descriptors).Count)
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