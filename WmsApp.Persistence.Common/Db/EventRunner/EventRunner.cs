using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class EventRunner : IEventRunner
    {
        public int RunEvents(DbContext context, Func<int> runBaseSaveChanges)
        {
            var eventsBefore = new Dictionary<EntityEventId, EntityEventValue>();
            var eventsAfter = new Dictionary<EntityEventId, EntityEventValue>();

            foreach (var entity in context.ChangeTracker.Entries<EntityWithEvents>())
            {
                entity.Entity.GetBeforeEventsAndClear(eventsBefore);
                entity.Entity.GetAfterEventsAndClear(eventsAfter);
            }

            var status = new StatusValidatorHandler();
            int rv = 0;


            using (var transaction = context.Database.CurrentTransaction ?? context.Database.BeginTransaction())
            {
                foreach (var entity in eventsBefore)
                    status.CombineStatues(context.HandleEntityEvent(entity.Value.Entity, entity.Value.Event));

                rv = runBaseSaveChanges();

                foreach (var entity in eventsAfter)
                    status.CombineStatues(context.HandleEntityEvent(entity.Value.Entity, entity.Value.Event));

                if (status.IsValid)
                    transaction.Commit();
            }

            return rv;

        }
    }
}

