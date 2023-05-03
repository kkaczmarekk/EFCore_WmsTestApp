using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Persistence.Common.Migrations
{
    internal class MigrationScriptQueryData
    {
        internal MigrationScriptQueryData(string migrationName, Assembly scriptAssembly, MigrationScriptResourceData resourceData) 
        {
            using (var stream = scriptAssembly.GetManifestResourceStream(resourceData.ToString()))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var data = ms.ToArray();
                    var sbQueryBuilder = new StringBuilder();
                    sbQueryBuilder.Append(GetHeader(migrationName, resourceData))
                        .Append(Encoding.UTF8.GetString(data, 3, data.Length - 3));

                    Query = sbQueryBuilder.ToString();
                    ResourceData = resourceData;
                }
            }


        }

        public string Query { get; }
        public MigrationScriptResourceData ResourceData { get; }

        private static string GetHeader(string migrationName, MigrationScriptResourceData resourceData) => $"/*\n" +
            $"SCRIPT GENERATED IN \"{migrationName}\" MIGRATION\n" +
            $"CODEBASE FROM \"{resourceData.SctiptName}.{resourceData.VersionStr}.{resourceData.ExtensionStr}\" EMBEDDED RESOURCE\n" +
            $"*/\n";
    }
}
