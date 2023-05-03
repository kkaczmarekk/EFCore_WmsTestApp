using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Domain.Items;

namespace WmsApp.Persistence.Items.Tests.Fake
{
    internal class ItemDbContextSqliteFake : ItemDbContext
    {
        public ItemDbContextSqliteFake(DbContextOptions<ItemDbContext> options) : base(options)
        {

        }


        public override int sp_UpdateCategoriesOwnedStringWithParents(int id)
        {
            var allCategories = Categories.ToList();

            var editCat = allCategories.Where(c => c.Id == id)
                .FirstOrDefault();

            if (editCat == null) return 0;

            Dictionary<Category, List<int>> catVisited = new Dictionary<Category, List<int>>();

            var catToUpdate = editCat;

            while (catToUpdate != null)
            {
                var categoriesOwnedIds = AppendCategoriesOwnedIds(catToUpdate, catVisited);
                DatabaseUpdateCategoriesOwnedString(catToUpdate.Id, categoriesOwnedIds);

                if(!catVisited.ContainsKey(catToUpdate))
                    catVisited.Add(catToUpdate, categoriesOwnedIds);

                catToUpdate = catToUpdate.ParentCategory;
            }

            return 0;
        }

        private void DatabaseUpdateCategoriesOwnedString(int catId, List<int> categoriesOwnedIds)
        {
            var sbCategoriesOwnedString = new StringBuilder();
            for (int i = 0; i < categoriesOwnedIds.Count; i++)
            {
                sbCategoriesOwnedString.Append(categoriesOwnedIds[i]);

                if (i < categoriesOwnedIds.Count - 1)
                    sbCategoriesOwnedString.Append(Category.CONST.OWNED_SEPARATOR);
            }

            var entityType = Model.FindEntityType(typeof(Category));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var tableNameWithSchema = (String.IsNullOrWhiteSpace(schema) ? "" : $"[{schema}]")
                + $"[{tableName}]";

            var sqlCommand = $"UPDATE {tableNameWithSchema}\n" +
                $"SET [CategoriesOwnedString] = '{sbCategoriesOwnedString.ToString()}'\n" +
                $"WHERE [Id] = {catId}";

            Database.ExecuteSqlRaw(sqlCommand);
        }

        private List<int> AppendCategoriesOwnedIds(Category cat, Dictionary<Category, List<int>> catVisited)
        {
            var rv = new List<int>();

            if (cat == null) return rv;

            if (catVisited.ContainsKey(cat)) return catVisited[cat];

            rv.AddRange(cat.CategoriesOwned.Select(c => c.Id));

            foreach (var child in cat.CategoriesOwned)
                rv.AddRange(AppendCategoriesOwnedIds(child, catVisited));

            return rv;
        }

        public override int sp_UpdateCategoryBranchStringWithChilds(int id)
        {
            var allCategories = Categories.ToList();

            var editCat = allCategories.Where(c => c.Id == id)
                .FirstOrDefault();

            if (editCat == null) return 0;

            var parentString = GetInitialParentBranchString(editCat);

            DatabaseUpdateCategoryBranchStringWithChilds(editCat, parentString);

            return 0;
        }

        private string GetInitialParentBranchString(Category editCat)
        {
            var sbInitialString = new StringBuilder();
            var parent = editCat.ParentCategory;

            while(parent != null)
            {
                if (sbInitialString.Length != 0)
                    sbInitialString.Insert(0, Category.CONST.BRANCH_SEPARATOR);

                sbInitialString.Insert(0, parent.Name);

                parent = parent.ParentCategory;
            }
            return sbInitialString.ToString();
        }

        private void DatabaseUpdateCategoryBranchStringWithChilds(Category cat, string parentString)
        {
            var sbCategoryBranchString = new StringBuilder();

            if(!string.IsNullOrEmpty(parentString))
                sbCategoryBranchString.Append(parentString)
                    .Append(Category.CONST.BRANCH_SEPARATOR);

            sbCategoryBranchString.Append(cat.Name);

            var entityType = Model.FindEntityType(typeof(Category));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var tableNameWithSchema = (String.IsNullOrWhiteSpace(schema) ? "" : $"[{schema}]")
                + $"[{tableName}]";

            var sqlCommand = $"UPDATE {tableNameWithSchema}\n" +
                $"SET [CategoryBranchString] = '{sbCategoryBranchString.ToString()}'\n" +
                $"WHERE [Id] = {cat.Id}";


            Database.ExecuteSqlRaw(sqlCommand);

            foreach(var child in cat.CategoriesOwned)
                DatabaseUpdateCategoryBranchStringWithChilds(child, sbCategoryBranchString.ToString());
        }


        //public static HashSet<int> CategoiresOwnedString_GetOwnedIds(
        //    this CategoryCashed categoryCashed) => Array
        //    .ConvertAll(categoryCashed.CategoriesOwnedString.Split(CategoryCashed.CONST.OWNED_SEPARATOR), int.Parse).ToHashSet();

        //public static IStatusValidator CashedCategoryBranchString_UpdateWithOwned(
        //    this Category cat)
        //{
        //    var status = new StatusValidatorHandler();
        //    var sbBranchString = new StringBuilder();
        //    var parentBranchString = cat.ParentCategory?.Cashed.CategoryBranchString ?? "";

        //    if (parentBranchString == "")
        //        sbBranchString.Append(cat.Name);
        //    else
        //        sbBranchString.Append(parentBranchString)
        //            .Append(CategoryCashed.CONST.BRANCH_SEPARATOR)
        //            .Append(cat.Name);

        //    status.CombineStatues(cat.Cashed.UpdateCategoryBranchData(sbBranchString.ToString()));

        //    foreach (var catOwned in cat.CategoriesOwned)
        //        status.CombineStatues(catOwned.CashedCategoryBranchString_UpdateWithOwned());

        //    return status;
        //}

        //public static IStatusValidator CashedCategoriesOwnedString_AddRangeWithParents(
        //    this Category parentCategory,
        //    HashSet<int> categoryIdsToAdd)
        //{
        //    var status = new StatusValidatorHandler();
        //    var sbNewCatsOwned = new StringBuilder().Append(parentCategory.CategoriesOwnedString);

        //    foreach (var cat in categoryIdsToAdd)
        //        sbNewCatsOwned.Append(cat)
        //            .Append(Category.CONST.OWNED_SEPARATOR);

        //    if (sbNewCatsOwned.Length > 0)
        //        sbNewCatsOwned.Remove(sbNewCatsOwned.Length - Category.CONST.OWNED_SEPARATOR.Length - 1, Category.CONST.OWNED_SEPARATOR.Length);

        //    status.CombineStatues(parentCategory.UpdateCategoriesOwnedData(sbNewCatsOwned.ToString()));

        //    return status.CombineStatues(parentCategory.ParentCategory?.CashedCategoriesOwnedString_AddRangeWithParents(categoryIdsToAdd));
        //}

        //public static IStatusValidator CashedCategoriesOwnedString_AddWithParents(
        //    this Category cat, 
        //    int newCategoryId)
        //{
        //    var status = new StatusValidatorHandler();

        //    var sbCategoriesOwned = new StringBuilder();

        //    if (cat.Cashed.CategoriesOwnedString.Length == 0)
        //        sbCategoriesOwned.Append(newCategoryId);
        //    else
        //        sbCategoriesOwned.Append(cat.Cashed.CategoriesOwnedString)
        //            .Append(CategoryCashed.CONST.OWNED_SEPARATOR)
        //            .Append(newCategoryId);

        //    status.CombineStatues(cat.Cashed.UpdateCategoriesOwnedData(sbCategoriesOwned.ToString()));

        //    return status.CombineStatues(cat.ParentCategory?.CashedCategoriesOwnedString_AddWithParents(newCategoryId));
        //}

        //public static IStatusValidator CashedCategoriesOwnedString_RemoveRangeWithParents(
        //    this Category parentCategory, 
        //    HashSet<int> categoryIdsToRemove)
        //{
        //    var status = new StatusValidatorHandler();
        //    var catsOwned = parentCategory.Cashed.CategoiresOwnedString_GetOwnedIds();

        //    var catAfterRemove = catsOwned.Except(categoryIdsToRemove);

        //    var sbNewCatsOwned = new StringBuilder();

        //    foreach (var cat in catAfterRemove)
        //        sbNewCatsOwned.Append(cat)
        //            .Append(CategoryCashed.CONST.OWNED_SEPARATOR);

        //    if (sbNewCatsOwned.Length > 0)
        //        sbNewCatsOwned.Remove(sbNewCatsOwned.Length - CategoryCashed.CONST.OWNED_SEPARATOR.Length - 1, CategoryCashed.CONST.OWNED_SEPARATOR.Length);

        //    status.CombineStatues(parentCategory.Cashed.UpdateCategoriesOwnedData(sbNewCatsOwned.ToString()));

        //    return status.CombineStatues(parentCategory.ParentCategory?.CashedCategoriesOwnedString_RemoveRangeWithParents(categoryIdsToRemove));
        //}

        //public static IStatusValidator CashedCategoriesOwnedString_RemoveWithParents(
        //    this Category parentCategory,
        //    int dellCategoryId)
        //{
        //    var status = new StatusValidatorHandler();
        //    var catsOwned = parentCategory.Cashed.CategoiresOwnedString_GetOwnedIds();
        //    catsOwned.Remove(dellCategoryId);

        //    var sbNewCatsOwned = new StringBuilder();

        //    foreach (var cat in catsOwned)
        //        sbNewCatsOwned.Append(cat)
        //            .Append(CategoryCashed.CONST.OWNED_SEPARATOR);

        //    if (sbNewCatsOwned.Length > 0)
        //        sbNewCatsOwned.Remove(sbNewCatsOwned.Length - CategoryCashed.CONST.OWNED_SEPARATOR.Length - 1, CategoryCashed.CONST.OWNED_SEPARATOR.Length);

        //    status.CombineStatues(parentCategory.Cashed.UpdateCategoriesOwnedData(sbNewCatsOwned.ToString()));

        //    return status.CombineStatues(parentCategory.ParentCategory?.CashedCategoriesOwnedString_RemoveWithParents(dellCategoryId));
        //}
    }
}
