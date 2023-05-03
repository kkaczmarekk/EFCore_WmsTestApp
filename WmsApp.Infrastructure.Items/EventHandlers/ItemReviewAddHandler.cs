using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;
using WmsApp.Domain.Items.DomainEvents;
using WmsApp.Domain.Items.Extensions;

namespace WmsApp.Infrastructure.Items.EventHandlers
{
    public class ItemReviewAddHandler : IEntityEventHandler<ItemReviewAddEvent>
    {
        public IStatusValidator Handle(object callingEntity, ItemReviewAddEvent entityEvent)
        {
            var status = new StatusValidatorHandler();

            var callingItem = callingEntity as Item;

            if (callingItem == null) return status.AddError($"Event should be called from {nameof(Item)} class.");

            status.CombineStatues(callingItem.Cashed.CashedReviewsDataRecalculate_Add(entityEvent.VotedStars));

            return status;
        }
    }
}
