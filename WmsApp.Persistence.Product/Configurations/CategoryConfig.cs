using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Items;

namespace WmsApp.Persistence.Items.Configurations
{
    internal class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            //return;
            entity.Property(c => c.CategoriesOwnedString)
                .ValueGeneratedOnAddOrUpdate();
            
            entity.Property(c => c.CategoryBranchString)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
