using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using WmsApp.Domain.Common.DateTimeGenerator;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Common.StatusValidator.Extensions;
using WmsApp.Domain.Items;
using WmsApp.Domain.Items.DomainEvents;
using WmsApp.Infrastructure.Items.EventHandlers;
using WmsApp.Persistence.Common.Db;
using WmsApp.Persistence.Items;
using WmsApp.Persistence.Common.Extensions;
using WmsApp.Persistence.Common.Migrations;
using System.Linq.Expressions;
using System.Xml;
using WmsApp.Persistence.Common.Db.EventRunner;

namespace EFCore_TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            EventRunnerConfig.SetEntityHandlersFromAssembly(Assembly.GetAssembly(typeof(CategoryBranchStringUpdateEventHandler)));
            using (var db = new DesignTimeContextFactory().CreateDbContext(null))
            {
                var cat = Category.CreateCategory("Fashion", null, null, new ActualDateTimeGenerator(), "TEST",
                    new List<Category>()
                    {
                    Category.CreateCategory("Fashion_Male", null, null, new ActualDateTimeGenerator(), "TEST",
                        new List<Category>()
                        {
                            Category.CreateCategory("Fashion_Male_Shoe", null, null, new ActualDateTimeGenerator(), "TEST").Result
                        }).Result,
                    Category.CreateCategory("Fashion_Female", null, null, new ActualDateTimeGenerator(), "TEST",
                        new List<Category>()
                        {
                            Category.CreateCategory("Fashion_Female_Shoe", null, null, new ActualDateTimeGenerator(), "TEST").Result
                        }).Result,
                    }).Result;

                db.Categories.Add(cat);
                db.Categories.Add(Category.CreateCategory("electronic", null, null, new ActualDateTimeGenerator(), "TEST").Result);

                db.SaveChanges();

            }
        }
    }
}