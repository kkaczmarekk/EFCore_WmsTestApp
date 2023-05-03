using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Persistence.Common.Migrations;

namespace WmsApp.Persistence.Common.Extensions
{
    public static class MigrationExtensions
    {
        public static void AppendScriptFromEmbedded(
            this MigrationBuilder migrationBuilder
            ,string migrationName
            ,MigrationScriptFilterBuilder scriptFilterBuilder
            ,Assembly scriptsAssembly = null)
        {
            var queriesData = new MigrationScriptsSqlHandler(migrationName,
                scriptsAssembly?? Assembly.GetCallingAssembly(), 
                scriptFilterBuilder);

            foreach (var scriptSql in queriesData.GetScriptsSql())
                migrationBuilder.Sql(scriptSql.Query);
        }
    }
}
