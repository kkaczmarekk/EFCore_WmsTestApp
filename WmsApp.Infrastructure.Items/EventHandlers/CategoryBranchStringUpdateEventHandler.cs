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
    public class CategoryBranchStringUpdateEventHandler : IEntityEventHandler<CategoryBranchStringUpdateEvent>
    {
        ItemDbContext _context;

        public CategoryBranchStringUpdateEventHandler(ItemDbContext context)
        {
            _context = context;
        }

        public IStatusValidator Handle(object callingEntity, CategoryBranchStringUpdateEvent entityEvent)
        {
            var status = new StatusValidatorHandler();

            var callingCategory = callingEntity as Category;

            if (callingCategory == null) return status.AddError($"Event should be called from {nameof(Category)} class.");

            _context.sp_Category_UpdateAllCategoryBranchStrings();

            return status;
        }
    }
}
