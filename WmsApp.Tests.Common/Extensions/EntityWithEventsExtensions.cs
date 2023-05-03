using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;

namespace WmsApp.Tests.Common.Extensions
{
    public static class EntityWithEventsExtensions
    {
        public static void ClearAllEntityEvents(
            this EntityWithEvents enity)
        {
            enity.GetAfterEventsAndClear();
            enity.GetBeforeEventsAndClear();
        }
    }
}
