using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;
using WmsApp.Domain.Items.DomainEvents;
using WmsApp.Persistence.Items;

namespace WmsApp.Infrastructure.Items.EventHandlers
{
    public class CategoryOwnedStringUpdateEventHandler : IEntityEventHandler<CategoryOwnedStringUpdateEvent>
    {
        ItemDbContext _context;

        public CategoryOwnedStringUpdateEventHandler(ItemDbContext context)
        {
            _context = context;
        }

        public IStatusValidator Handle(object callingEntity, CategoryOwnedStringUpdateEvent entityEvent)
        {
            var status = new StatusValidatorHandler();

            var callingCategory = callingEntity as Category;

            if (callingCategory == null) return status.AddError($"Event should be called from {nameof(Category)} class.");

            _context.sp_Category_UpdateAllCategoriesOwnedStrings();

            return status;
        }
    }
}
