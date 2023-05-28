using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;
using WmsApp.Domain.Items.Tests.Unit;
using WmsApp.Infrastructure.Items.EventHandlers;
using WmsApp.Persistence.Common.Db;
using WmsApp.Persistence.Common.Db.EventRunner;
using WmsApp.Persistence.Items.Tests.Fake;
using WmsApp.Tests.Common.Factories.Items;

namespace WmsApp.Persistence.Items.Tests.Integration.SqlLite
{
    public class CategoryTests
    {
        [Fact]
        public void CreateCategory_ParentOwnedCategoiesNotLoaded()
        {
            //SETUP
            EventRunnerConfig.SetEntityHandlersFromAssembly(Assembly.GetAssembly(typeof(CategoryBranchStringUpdateEventHandler)));
            using var con = new SqliteInMemory<ItemDbContextSqliteFake, ItemDbContext>();

            var complexCategory = new CategoryFactory.FashionCompexCategory();
            var newCat = new CategoryFactory.SimpleCategory(0, complexCategory.Fashion_Female_Shoe);

            using (var context = con.GetContext())
            {
                context.Database.EnsureCreated();
                context.Categories.Add(complexCategory.Fashion);
                context.SaveChanges();
            }



            //ATTEMPT
            var catTestResult = new StatusValidatorHandler();
            using (var context = con.GetContext())
            {
                var catTest = context.Categories
                    .ToList();
            }

            //VERIFY

        }
    }
}
