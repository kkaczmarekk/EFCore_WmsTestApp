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
                () => Udf_CategoriesBranchString(default(int), string.Empty));

            modelBuilder.HasDbFunction(
                () => Udf_CategoriesOwnedString(default(int), string.Empty));
        }

        public static string Udf_CategoriesBranchString(int catId, string separator)
        {
            return null;
        }

        public static string Udf_CategoriesOwnedString(int catId, string separator)
        {
            return null;
        }
    }
}
