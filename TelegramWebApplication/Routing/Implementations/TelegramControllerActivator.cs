using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramWebApplication.Routing
{
    public class TelegramControllerActivator : ITelegramControllerActivator
    {
        private IServiceProvider _provider;
        private ITelegramRouteTree _telegramRouteTree;

        public TelegramControllerActivator(ITelegramRouteTree routeTree, IServiceProvider provider)
        {
            _telegramRouteTree = routeTree;
            _provider = provider;
        }

        public object? GetControllerForUpdateAsync(Update update)
        {
            ITelegramRouteDescriptor? descriptor = _telegramRouteTree.Resolve(update);

            if (descriptor is null)
            {
                return null;
            }

            ConstructorInfo constructor = descriptor.ControllerType!.GetConstructors().First();
            ParameterInfo[] parameters = constructor.GetParameters();

            List<object> readyParams = new List<object>();

            foreach (ParameterInfo parameter in parameters)
            {
                Type serviceType = parameter.GetType();
                object? service = _provider.GetService(serviceType);

                if (service is null)
                {
                    throw new Exception(
                        $"Unable to resolve service for type {serviceType.FullName} " +
                        $"while attempting to activate '{descriptor.ControllerType.FullName}'."
                    );
                }

                readyParams.Add(service);
            }

            object controller = constructor.Invoke(readyParams.ToArray());

            return controller;
        }
    }
}