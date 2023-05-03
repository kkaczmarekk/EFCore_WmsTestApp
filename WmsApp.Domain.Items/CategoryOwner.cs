using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Items
{
    
    public partial class CategoryOwner
    {
        private CategoryOwner() { }

        public CategoryOwner(Category category, Owner owner, OwnerType type)
        {
            Category = category;
            Owner = owner;
            Type = type;
        }

        public IStatusValidator Type_Update(
            OwnerType type)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(type, nameof(Type));

            if (!status.IsValid) return status;

            Type = type;

            return status;
        }
    }
}
