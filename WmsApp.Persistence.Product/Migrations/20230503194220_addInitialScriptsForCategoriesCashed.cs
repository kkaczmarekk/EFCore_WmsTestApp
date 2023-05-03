using Microsoft.EntityFrameworkCore.Migrations;
using WmsApp.Persistence.Common.Migrations;
using WmsApp.Persistence.Common.Extensions;
#nullable disable

namespace WmsApp.Persistence.Items.Migrations
{
    /// <inheritdoc />
    public partial class addInitialScriptsForCategoriesCashed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var scriptsFilter = new MigrationScriptFilterBuilder()
                .AddFunction("udf_CategoriesOwnedString", "v0")
                .AddFunction("udf_CategoryBranchString", "v0")
                .AddStoredProcedure("sp_UpdateCategoriesOwnedStringWithParents", "v0")
                .AddStoredProcedure("sp_UpdateCategoryBranchStringWithChilds", "v0");

            migrationBuilder.AppendScriptFromEmbedded("initialMigration", scriptsFilter);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
