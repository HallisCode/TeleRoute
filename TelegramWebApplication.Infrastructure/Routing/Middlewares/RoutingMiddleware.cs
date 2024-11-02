using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleNetFramework.Core.Middleware;
using Telegram.Bot.Types;
using TelegramWebApplication.Core.Routing;

namespace TelegramWebApplication.Infrastructure.Routing.Middlewares
{
    /// <summary>
    /// Вызывает контроллер для обработки текущего update. Не вызыает последующий обработчик middleware.
    /// </summary>
    public class RoutingMiddleware : IMiddleware<Update>
    {
        private readonly ITelegramRouteTree _telegramRouteTree;
        private readonly IServiceProvider _serviceProvider;


        public MiddlewareDelegate<Update> Next { get; }


        public RoutingMiddleware(ITelegramRouteTree telegramRouteTree, IServiceProvider provider)
        {
            _telegramRouteTree = telegramRouteTree;
            _serviceProvider = provider;
        }

        public async Task Invoke(Update update)
        {
            ITelegramRouteDescriptor? telegramRouteDescriptor = _telegramRouteTree.Resolve(update);

            if (telegramRouteDescriptor is null) return;

            ConstructorInfo constructor = telegramRouteDescriptor.ControllerType!.GetConstructors().First();

            List<object> resolvedServices = new List<object>();
            foreach (Type neededService in telegramRouteDescriptor.NeededTypesForController)
            {
                object? service = _serviceProvider.GetService(neededService);

                if (service is null)
                {
                    throw new Exception(
                        $"Unable to resolve service for type '{neededService.FullName}' " +
                        $"while attempting to activate '{telegramRouteDescriptor.ControllerType.FullName}'."
                    );
                }

                resolvedServices.Add(service);
            }

            // Порождаем контроллер
            object controller;
            if (telegramRouteDescriptor.NeededTypesForController.Length == 0)
            {
                controller = Activator.CreateInstance(telegramRouteDescriptor.ControllerType)!;
            }
            else
            {
                controller = constructor.Invoke(resolvedServices.ToArray());
            }

            // Запускаем обработчик 
            Task handling = (Task)telegramRouteDescriptor.ControllerType
                .GetMethod(telegramRouteDescriptor.Handler!.Name)?
                .Invoke(controller, [update])!;


            await handling;

            return;
        }
    }
}