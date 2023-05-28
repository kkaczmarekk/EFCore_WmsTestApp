using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Persistence.Items
{
    public static class UdfDefinitions
    {
        public static void RegisterUdfDefinitions(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(
                () => udf_Category_GetCategoriesOwnedString(default(int), string.Empty));

            modelBuilder.HasDbFunction(
                () => udf_Category_GetCategoryBranchString(default(int), string.Empty));
        }

        public static string udf_Category_GetCategoriesOwnedString(int catId, string separator)
        {
            return null;
        }

        public static string udf_Category_GetCategoryBranchString(int catId, string separator)
        {
            return null;
        }
    }
}
