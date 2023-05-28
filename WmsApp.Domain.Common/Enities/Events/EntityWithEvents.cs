using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Events
{
    public enum EventRunType { Before,  After }
    public enum EventRunScope { Transient, Entity, Context }

    public struct EntityEventId
    {
        public EntityEventId(Guid entityGuid, Guid methodGuid)
        {
            EntityGuid = entityGuid;
            MethodGuid = methodGuid;
        }

        public Guid EntityGuid { get; }
        public Guid MethodGuid { get; }
    }

    public struct EntityEventValue
    {
        public EntityEventValue(EntityWithEvents entity, IEntityEvent entityEvent)
        {
            Entity = entity;
            Event = entityEvent;
        }
        public EntityWithEvents Entity { get; }
        public IEntityEvent Event { get;  }
    }

    public abstract class EntityWithEvents
    {
        private Guid _entityScopeGuid = Guid.NewGuid();
        private static Guid _contextScopeGuid = Guid.NewGuid();

        private readonly Dictionary<EntityEventId, EntityEventValue> _eventsBeforeSave = new Dictionary<EntityEventId, EntityEventValue>();
        private readonly Dictionary<EntityEventId, EntityEventValue> _eventsAfterSave = new Dictionary<EntityEventId, EntityEventValue>();

        public void AddEvent(EventRunType runType, EventRunScope scopeType, IEntityEvent entityEvent)
        {
            var eventGuid = GetGuidFromScope(scopeType, _entityScopeGuid, entityEvent);

            if(_eventsBeforeSave.ContainsKey(eventGuid) 
                || _eventsAfterSave.ContainsKey(eventGuid))
            {
                return;
            }

            switch (runType)
            {
                case EventRunType.Before:
                    _eventsBeforeSave.Add(eventGuid, new EntityEventValue(this, entityEvent));
                    break;
                case EventRunType.After:
                    _eventsAfterSave.Add(eventGuid, new EntityEventValue(this, entityEvent));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(runType.ToString());
            }
        }


        private static EntityEventId GetGuidFromScope(EventRunScope scopedType
            , Guid entityGuid
            , IEntityEvent entityEvent) => scopedType switch
        {
            EventRunScope.Transient => new EntityEventId (Guid.NewGuid(), entityEvent.GetGuid()),
            EventRunScope.Entity => new EntityEventId(entityGuid, entityEvent.GetGuid()),
            EventRunScope.Context => new EntityEventId(_contextScopeGuid, entityEvent.GetGuid()),
            _ => throw new ArgumentOutOfRangeException(scopedType.ToString())
        };

        public IDictionary<EntityEventId, EntityEventValue> GetBeforeEventsAndClear(IDictionary<EntityEventId, EntityEventValue> eventsToHandle = null)
        {
            if (eventsToHandle is null)
            {
                var newEventsTohandle = _eventsBeforeSave.ToDictionary(events => events.Key,
                    events => events.Value);

                _eventsBeforeSave.Clear();
                return newEventsTohandle;
            }  

            foreach (var entityEvent in _eventsBeforeSave)
                if (!eventsToHandle.ContainsKey(entityEvent.Key))
                    eventsToHandle.Add(entityEvent.Key, entityEvent.Value);

            _eventsBeforeSave.Clear();
            return eventsToHandle;
        }

        public IDictionary<EntityEventId, EntityEventValue> GetAfterEventsAndClear(IDictionary<EntityEventId, EntityEventValue> eventsToHandle = null)
        {
            if (eventsToHandle is null)
            {
                var newEventsTohandle = _eventsAfterSave.ToDictionary(events => events.Key,
                    events => events.Value);

                _eventsAfterSave.Clear();
                return newEventsTohandle;
            }

            foreach (var entityEvent in _eventsAfterSave)
                if (!eventsToHandle.ContainsKey(entityEvent.Key))
                    eventsToHandle.Add(entityEvent.Key, entityEvent.Value);

            _eventsAfterSave.Clear();
            return eventsToHandle;
        }
    }
}
