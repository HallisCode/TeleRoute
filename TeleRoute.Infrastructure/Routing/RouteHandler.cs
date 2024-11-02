using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramWebApplication.Core.Routing;

namespace TelegramWebApplication.Infrastructure.Routing
{
    public class RouteHandler : IRouteHandler
    {
        private readonly IRouteTree _routeTree;
        private readonly IServiceProvider _serviceProvider;
        

        public RouteHandler(IRouteTree routeTree, IServiceProvider provider)
        {
            _routeTree = routeTree;
            _serviceProvider = provider;
        }
        
        
        public async Task Handle(Update update)
        {
            IRouteDescriptor? routeDescriptor = _routeTree.Resolve(update);

            if (routeDescriptor is null) return;

            ConstructorInfo constructor = routeDescriptor.ControllerType!.GetConstructors().First();

            // Получаем зависимости для конструтора
            List<object> resolvedServices = new List<object>();
            foreach (Type neededService in routeDescriptor.NeededTypesForController!)
            {
                object? service = _serviceProvider.GetService(neededService);

                if (service is null)
                {
                    throw new Exception(
                        $"Unable to resolve service for type '{neededService.FullName}' " +
                        $"while attempting to activate '{routeDescriptor.ControllerType.FullName}'."
                    );
                }

                resolvedServices.Add(service);
            }

            // Порождаем контроллер
            object controller;
            if (routeDescriptor.NeededTypesForController.Length == 0)
            {
                controller = Activator.CreateInstance(routeDescriptor.ControllerType)!;
            }
            else
            {
                controller = constructor.Invoke(resolvedServices.ToArray());
            }

            // Запускаем обработчик 
            Task handling = (Task)routeDescriptor.ControllerType
                .GetMethod(routeDescriptor.Handler!.Name)?
                .Invoke(controller, [update])!;
            
            await handling;
        }
    }
}