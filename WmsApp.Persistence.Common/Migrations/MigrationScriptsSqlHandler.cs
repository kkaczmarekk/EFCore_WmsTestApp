using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Persistence.Common.Migrations
{
    public class MigrationScriptsSqlHandler
    {
        private Dictionary<string, MigrationScriptQueryData> _migrationScriptsData;
        public MigrationScriptsSqlHandler(string migrationName, Assembly scriptsAssembly, MigrationScriptFilterBuilder scriptsType) 
        {
            var allResourceNames = scriptsAssembly.GetManifestResourceNames();

            _migrationScriptsData = new Dictionary<string, MigrationScriptQueryData>();

            var queryFilter = scriptsType.Build();

            foreach (var resourceName in allResourceNames)
            {
                var scriptData = new MigrationScriptResourceData(resourceName);

                if (queryFilter(scriptData))
                    AddOrUpdateScriptInDic(scriptData, migrationName, scriptsAssembly);
            }
            MigrationName = migrationName;
        }

        private void AddOrUpdateScriptInDic(MigrationScriptResourceData scriptData, string migrationName, Assembly scriptsAssembly)
        {
            if (_migrationScriptsData.ContainsKey(scriptData.SctiptName) 
                && scriptData > _migrationScriptsData[scriptData.SctiptName].ResourceData)
            {
                _migrationScriptsData[scriptData.SctiptName] = new MigrationScriptQueryData(migrationName, scriptsAssembly, scriptData);
                return;
            }

            if(!_migrationScriptsData.ContainsKey(scriptData.SctiptName))
                _migrationScriptsData.Add(scriptData.SctiptName, new MigrationScriptQueryData(migrationName, scriptsAssembly, scriptData));
        }


        internal IReadOnlyCollection<MigrationScriptQueryData> GetScriptsSql()
        {
            return _migrationScriptsData.Values.ToList();
        }

        public string MigrationName { get; }
    }
}
