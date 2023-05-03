using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;

namespace WmsApp.Persistence.Common.Db
{
    public abstract class EventsDbContext<T> : DbContext
        where T : DbContext
    {
        private static Dictionary<Type, Type> _entityEventsWithHandlers = new Dictionary<Type, Type>();

        public EventsDbContext(DbContextOptions<T> options) : base(options)
        {

        }

        public EventsDbContext() : base()
        {

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            List<(EntityWithEvents, ICollection<IEntityEvent>)> entityEventsBefore = new List<(EntityWithEvents, ICollection<IEntityEvent>)>();
            List<(EntityWithEvents, ICollection<IEntityEvent>)> entityEventsAfter = new List<(EntityWithEvents, ICollection<IEntityEvent>)>();

            foreach (var entity in ChangeTracker.Entries<EntityWithEvents>())
            {
                UpdateEvents(entityEventsBefore, entity.Entity, entity.Entity.GetBeforeEventsAndClear);
                UpdateEvents(entityEventsAfter, entity.Entity, entity.Entity.GetAfterEventsAndClear);
            }

            var status = new StatusValidatorHandler();
            int rv = 0;


            using (var transaction = Database.BeginTransaction())
            {
                foreach (var entity in entityEventsBefore)
                    foreach (var entityEvent in entity.Item2)
                        status.CombineStatues(HandleEntityEvent(entity.Item1, entityEvent));

                rv = base.SaveChanges(acceptAllChangesOnSuccess);

                foreach (var entity in entityEventsAfter)
                    foreach (var entityEvent in entity.Item2)
                        status.CombineStatues(HandleEntityEvent(entity.Item1, entityEvent));

                transaction.Commit();
            }

            return rv;
        }

        private IStatusValidator HandleEntityEvent(EntityWithEvents entity, IEntityEvent entityEvent)
        {
            var hanlder = _entityEventsWithHandlers[entityEvent.GetType()];
            var handlerIstance = Activator.CreateInstance(hanlder, this);
            var rv = hanlder.GetMethod("Handle").Invoke(handlerIstance, new object[] { entity, entityEvent });

            return rv as IStatusValidator;
        }

        private void UpdateEvents(
            List<(EntityWithEvents, ICollection<IEntityEvent>)> allEvents,
            EntityWithEvents entity,
            Func<ICollection<IEntityEvent>> funcGetAndClear)
        {
            var events = funcGetAndClear();

            if (events.Count > 0)
                allEvents.Add((entity, events));
        }


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

    }
}
