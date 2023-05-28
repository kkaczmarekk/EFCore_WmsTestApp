using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using WmsApp.Persistence.Common.Extensions;
using WmsApp.Persistence.Common.Migrations;
#nullable disable

namespace WmsApp.Persistence.Items.Migrations
{
    /// <inheritdoc />
    public partial class add_CategoryUdfAndSpForCashedValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var scriptsFilter = new MigrationScriptFilterBuilder()
                .AddFunction("udf_Category_GetCategoriesOwnedString", "v0")
                .AddFunction("udf_Category_GetCategoryBranchString", "v0")
                .AddStoredProcedure("sp_Category_UpdateAllCategoriesOwnedStrings", "v0")
                .AddStoredProcedure("sp_Category_UpdateAllCategoryBranchStrings", "v0");

            migrationBuilder.AppendScriptFromEmbedded("add_CategoryUdfAndSpForCashedValues", scriptsFilter);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
