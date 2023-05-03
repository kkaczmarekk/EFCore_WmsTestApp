using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;
using WmsApp.Domain.Items.DomainEvents;
using WmsApp.Domain.Items.Extensions;
using WmsApp.Persistence.Items;

namespace WmsApp.Infrastructure.Items.EventHandlers
{
    public class CategoryAddedHandler : IEntityEventHandler<CategoryAddedEvent>
    {
        ItemDbContext _context;

        public CategoryAddedHandler(ItemDbContext context)
        {
            _context = context;
        }

        public IStatusValidator Handle(object callingEntity, CategoryAddedEvent entityEvent)
        {
            var status = new StatusValidatorHandler();

            var callingCategory = callingEntity as Category;

            if (callingCategory == null) return status.AddError($"Event should be called from {nameof(Category)} class.");

            _context.sp_UpdateCategoriesOwnedStringWithParents(callingCategory.Id);
            _context.sp_UpdateCategoryBranchStringWithChilds(callingCategory.Id);

            return status;
        }
    }
}
