using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Items;

namespace WmsApp.Persistence.Items.Configurations
{
    internal class CategoryOwnerConfig : IEntityTypeConfiguration<CategoryOwner>
    {
        public void Configure(EntityTypeBuilder<CategoryOwner> entity)
        {
            entity.HasKey(k => new { k.OwnerId, k.CategoryId });

            entity.HasQueryFilter(co => !co.Owner.IsDeleted && !co.Category.IsDeleted);
        }
    }
}
