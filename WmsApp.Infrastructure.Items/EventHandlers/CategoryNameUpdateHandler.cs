using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class CategoryNameUpdateHandler : IEntityEventHandler<CategoryNameUpdateEvent>
    {
        ItemDbContext _context;

        public CategoryNameUpdateHandler(ItemDbContext context)
        {
            _context = context;
        }

        public IStatusValidator Handle(object callingEntity, CategoryNameUpdateEvent entityEvent)
        {
            var status = new StatusValidatorHandler();

            var callingCategory = callingEntity as Category;

            if (callingCategory == null) return status.AddError($"Event should be called from {nameof(Category)} class.");

            _context.sp_UpdateCategoryBranchStringWithChilds(callingCategory.Id);

            return status;
        }
    }
}
