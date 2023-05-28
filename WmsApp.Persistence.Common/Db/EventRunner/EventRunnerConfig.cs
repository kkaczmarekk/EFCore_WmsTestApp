using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;

namespace WmsApp.Persistence.Common.Db.EventRunner
{
    public static class EventRunnerConfig
    {
        private static Dictionary<Type, Type> _entityEventsWithHandlers = new Dictionary<Type, Type>();

        public static void SetEntityHandlersFromAssembly(Assembly handlersAssembly)
        {
            var getEventsWithHandlers = handlersAssembly
                .GetExportedTypes()
                .Select(handler =>
                {
                    var entityEvent = handler.GetInterfaces()
                        .Where(i => i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IEntityEventHandler<>))
                        .SingleOrDefault()
                        .GetGenericArguments()
                        .FirstOrDefault();
                    return (handler, entityEvent);
                })
                .Where(eh => eh.entityEvent is not null);

            foreach (var eventWithHandler in getEventsWithHandlers)
                _entityEventsWithHandlers.Add(eventWithHandler.entityEvent, eventWithHandler.handler);
        }

        public static IStatusValidator HandleEntityEvent(this DbContext context
            , EntityWithEvents entity
            , IEntityEvent entityEvent)
        {
            var hanlder = _entityEventsWithHandlers[entityEvent.GetType()];
            var handlerIstance = Activator.CreateInstance(hanlder, context);
            var rv = hanlder.GetMethod("Handle").Invoke(handlerIstance, new object[] { entity, entityEvent });

            return rv as IStatusValidator;
        }
    }
}
