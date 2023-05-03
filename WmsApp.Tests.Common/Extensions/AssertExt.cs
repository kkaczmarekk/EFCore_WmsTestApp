using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using Xunit;

namespace WmsApp.Tests.Common.Extensions
{
    public static class AssertExt
    {
        public static void StatusValidator_OK(IStatusValidator status)
        {
            Assert.True(status.IsValid);
            Assert.Equal(0, status.Errors.Count);
        }

        public static void StatusValidator_Fail(IStatusValidator status)
        {
            Assert.False(status.IsValid);
            Assert.True(status.Errors.Count > 0);
        }

        public static void EntityEvents_Check(
            EntityWithEvents entity,
            int beforeCount = 0,
            int afterCount = 0)
        {
            var eventsBefore = entity.GetBeforeEventsAndClear();
            var eventsAfter = entity.GetAfterEventsAndClear();

            Assert.Equal(beforeCount, eventsBefore.Count);
            Assert.Equal(afterCount, eventsAfter.Count);
        }
    }
}