using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WmsApp.Domain.Items;
using WmsApp.Persistence.Items.Configurations;
using Microsoft.Data.SqlClient;
using WmsApp.Persistence.Common.Extensions;
using WmsApp.Persistence.Common.Db;

namespace WmsApp.Persistence.Items
{
    public class ItemDbContext : EventsDbContext<ItemDbContext>
    {
        private static IConfigurationRoot _configuration;
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Owner> Owners { get; set; }

        public ItemDbContext() { }
        
        public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationForCommonInterfaces();

            modelBuilder.ApplyConfiguration<Category>(new CategoryConfig());
            modelBuilder.ApplyConfiguration<CategoryOwner>(new CategoryOwnerConfig());
        }

        public virtual int sp_UpdateCategoriesOwnedStringWithParents(int id)
        {
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[sp_UpdateCategoriesOwnedStringWithParents] @CatId, @Separator"
                , new SqlParameter("@CatId", id)
                , new SqlParameter("@Separator", Category.CONST.OWNED_SEPARATOR));
        }

        public virtual int sp_UpdateCategoryBranchStringWithChilds(int id)
        {
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[sp_UpdateCategoryBranchStringWithChilds] @CatId, @Separator"
                , new SqlParameter("@CatId", id)
                , new SqlParameter("@Separator", Category.CONST.BRANCH_SEPARATOR));
        }
    }
}