using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;
using WmsApp.Persistence.Common.Db.EventRunner;

namespace WmsApp.Persistence.Items.Tests.Fake
{
    internal class ItemDbContextSqliteFake : ItemDbContext
    {
        public ItemDbContextSqliteFake(DbContextOptions<ItemDbContext> options
            , IEventRunner eventRunner)
        : base(options, eventRunner)
        {

        }

        public override int sp_Category_UpdateAllCategoriesOwnedStrings()
        {
            var rootCats = Categories.Where(c => c.ParentCategory == null)
                .ToList();

            var entityType = Model.FindEntityType(typeof(Category));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var tableNameWithSchema = (String.IsNullOrWhiteSpace(schema) ? "" : $"[{schema}]")
                + $"[{tableName}]";

            foreach (var cat in rootCats)
            {
                DatabaseUpdateCategoriesOwnedStringWithChilds(cat, tableNameWithSchema);
            }
            return 0;
        }

        private List<int> DatabaseUpdateCategoriesOwnedStringWithChilds(Category cat, string tableNameWithSchema)
        {
            List<int> ownedIds = new List<int>();

            foreach (var child in cat.CategoriesOwned)
            {
                ownedIds.Add(child.Id);
                ownedIds.AddRange(DatabaseUpdateCategoriesOwnedStringWithChilds(child, tableNameWithSchema));
            }

            StringBuilder sbCategoriesOwnedString = GetOwnedString(ownedIds);

            var sqlCommand = $"UPDATE {tableNameWithSchema}\n" +
                $"SET [CategoriesOwnedString] = '{sbCategoriesOwnedString.ToString()}'\n" +
                $"WHERE [Id] = {cat.Id}";

            Database.ExecuteSqlRaw(sqlCommand);

            return ownedIds;
        }

        private static StringBuilder GetOwnedString(List<int> ownedIds)
        {
            var sbCategoriesOwnedString = new StringBuilder();
            for (int i = 0; i < ownedIds.Count; i++)
            {
                sbCategoriesOwnedString.Append(ownedIds[i]);

                if (i < ownedIds.Count - 1)
                    sbCategoriesOwnedString.Append(Category.CONST.OWNED_SEPARATOR);
            }

            return sbCategoriesOwnedString;
        }

        public override int sp_Category_UpdateAllCategoryBranchStrings()
        {
            var rootCats = Categories.Where(c => c.ParentCategory == null)
                .ToList();

            var entityType = Model.FindEntityType(typeof(Category));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var tableNameWithSchema = (String.IsNullOrWhiteSpace(schema) ? "" : $"[{schema}]")
                + $"[{tableName}]";


            foreach (var cat in rootCats)
                DatabaseUpdateCategoryBranchStringWithChilds(cat, "", tableNameWithSchema);

            return 0;
        }

        private void DatabaseUpdateCategoryBranchStringWithChilds(Category cat
            , string parentString
            , string tableNameWithSchema)
        {
            var sbCategoryBranchString = new StringBuilder();

            if(!string.IsNullOrEmpty(parentString))
                sbCategoryBranchString.Append(parentString)
                    .Append(Category.CONST.BRANCH_SEPARATOR);

            sbCategoryBranchString.Append(cat.Name);

            var sqlCommand = $"UPDATE {tableNameWithSchema}\n" +
                $"SET [CategoryBranchString] = '{sbCategoryBranchString.ToString()}'\n" +
                $"WHERE [Id] = {cat.Id}";


            Database.ExecuteSqlRaw(sqlCommand);

            foreach(var child in cat.CategoriesOwned)
                DatabaseUpdateCategoryBranchStringWithChilds(child, sbCategoryBranchString.ToString(), tableNameWithSchema);
        }
    }
}
