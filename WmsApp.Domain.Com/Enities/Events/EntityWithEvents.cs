using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Events
{
    public enum EventRunType { Before,  After, AfterTrans }

    public abstract class EntityWithEvents
    {
        private readonly List<IEntityEvent> _eventsBeforeSave = new List<IEntityEvent>();
        private readonly List<IEntityEvent> _eventsAfterSafe = new List<IEntityEvent>();

        public void AddEvent(EventRunType runType, IEntityEvent entityEvent)
        {
            switch(runType)
            {
                case EventRunType.Before:
                    _eventsBeforeSave.Add(entityEvent);
                    break;
                case EventRunType.After:
                    _eventsAfterSafe.Add(entityEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(runType.ToString());
            }
        }

        public ICollection<IEntityEvent> GetBeforeEventsAndClear()
        {
            var result = _eventsBeforeSave.ToList();
            _eventsBeforeSave.Clear();
            return result;
        }

        public ICollection<IEntityEvent> GetAfterEventsAndClear()
        {
            var result = _eventsAfterSafe.ToList();
            _eventsAfterSafe.Clear();
            return result;
        }
    }
}
