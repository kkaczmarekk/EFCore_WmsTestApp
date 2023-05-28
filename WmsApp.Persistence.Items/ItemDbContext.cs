using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WmsApp.Domain.Items;
using WmsApp.Persistence.Items.Configurations;
using Microsoft.Data.SqlClient;
using WmsApp.Persistence.Common.Extensions;
using WmsApp.Persistence.Common.Db;
using WmsApp.Persistence.Common.Db.EventRunner;

namespace WmsApp.Persistence.Items
{
    public class ItemDbContext : EventsDbContext<ItemDbContext>
    {
        private static IConfigurationRoot _configuration;
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Owner> Owners { get; set; }

        public ItemDbContext() { }
        
        public ItemDbContext(DbContextOptions<ItemDbContext> options
            , IEventRunner eventRunner) 
        : base(options, eventRunner)
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

        public virtual int sp_Category_UpdateAllCategoriesOwnedStrings()
        {
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[sp_Category_UpdateAllCategoriesOwnedStrings] @Separator"
                , new SqlParameter("@Separator", Category.CONST.OWNED_SEPARATOR));
        }

        public virtual int sp_Category_UpdateAllCategoryBranchStrings()
        {
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[sp_Category_UpdateAllCategoryBranchStrings] @Separator"
                , new SqlParameter("@Separator", Category.CONST.BRANCH_SEPARATOR));
        }
    }
}