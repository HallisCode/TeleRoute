using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramBotClientExtended.Routing.Attributes;
using TelegramBotClientExtended.Routing.Filters;

namespace TelegramBotClientExtended.Routing
{
    public class TelegramRouteBuilder : ITelegramRouteBuilder
    {
        private List<ITelegramRouteDescriptor> _routes;

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
            Type[] types = assembly.GetTypes();

            Type[] classTypeWithAttribute = types.Where(
                (type) => type.IsClass && Attribute.IsDefined(type, typeof(TelegramRouteAttribute))
            ).ToArray();

            List<ITelegramRouteDescriptor> descriptors = new List<ITelegramRouteDescriptor>();
            foreach (Type classType in classTypeWithAttribute)
            {
                // Получаем все методы класса
                MethodInfo[] methods = classType.GetMethods();
                if (methods.Length <= 0)
                {
                    continue;
                }

                // Получаем все методы класса с аттрибутом TelegramRouteAttribute
                MethodInfo[] methodsWithAttribute = methods.Where(
                    (method) => Attribute.IsDefined(method, typeof(TelegramRouteAttribute))
                ).ToArray();

                // Получаем TelegramRouteAttribute у класса
                TelegramRouteAttribute? classRouteAttribute = (TelegramRouteAttribute)
                    Attribute.GetCustomAttribute(classType, typeof(TelegramRouteAttribute))!;

                // Проверяем какие условия заданы у класса
                bool isAllowedTypeDefined = classRouteAttribute.AllowedType is not null;
                bool isFilterDefined = classRouteAttribute.Filter is not null;

                bool isClassHasConditions = isAllowedTypeDefined || isFilterDefined;

                List<ITelegramRouteDescriptor> methodsDescriptors = _GetRoutesFromMethods(methodsWithAttribute);

                if (isClassHasConditions)
                {
                    TelegramRouteDescriptor routeDescriptor = new TelegramRouteDescriptor(
                        innerBranch: methodsDescriptors,
                        allowedTypes: classRouteAttribute.AllowedType,
                        filter: classRouteAttribute.Filter);

                    descriptors.Add(routeDescriptor);
                }
                else
                {
                    descriptors.AddRange(methodsDescriptors);
                }
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

                if (methodRouteAttribute is null)
                {
                    throw new Exception($"У метода {method.Module.Assembly.Location} отсутствует аттрибут " +
                                        $"{typeof(TelegramEndpointDelegate).FullName}");
                }

                TelegramRouteDescriptor routeDescriptor = new TelegramRouteDescriptor(
                    handler: (TelegramEndpointDelegate)method.CreateDelegate(typeof(TelegramEndpointDelegate)),
                    allowedTypes: methodRouteAttribute.AllowedType,
                    filter: methodRouteAttribute.Filter);

                descriptors.Add(routeDescriptor);
            }

            _VerifyUnduplicated(descriptors);

            return descriptors;
        }

        private void _VerifyMatchTelegramEndpointDelegate(MethodInfo method)
        {
            bool isMatch = Delegate.CreateDelegate(
                typeof(TelegramEndpointDelegate), method, false
            ) != null;

            if (isMatch is false)
            {
                throw new Exception($"К методу {method.Module.Assembly.Location} " +
                                    $"применён аттрибут {typeof(TelegramEndpointDelegate).FullName}, " +
                                    $"хотя он не соответствует делегату {typeof(TelegramEndpointDelegate).FullName}");
            }
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