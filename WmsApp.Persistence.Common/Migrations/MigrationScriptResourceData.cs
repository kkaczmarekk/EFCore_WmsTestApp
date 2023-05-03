using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Persistence.Common.Migrations
{
    internal class MigrationScriptResourceData
    {
        internal MigrationScriptResourceData(string manifestResourceName) 
        {
            var scriptData = manifestResourceName.Split('.');

            ExtensionStr = scriptData[scriptData.Length - 1];
            VersionStr = scriptData[scriptData.Length - 2];
            SctiptName = scriptData[scriptData.Length - 3];
            SctiptType = scriptData[scriptData.Length - 5];

            var sbPathData = new StringBuilder();
            for(int i = 0; i < scriptData.Length - 5; i++)
                sbPathData.Append(scriptData[i])
                    .Append(".");
            PathData = sbPathData.ToString();
        }

        public string ExtensionStr { get; }
        public string VersionStr { get; }
        public string SctiptName { get; }
        public string SctiptType { get; }
        public string PathData { get; }

        public override string ToString()
        {
            var sbToStringBuilder = new StringBuilder();

            sbToStringBuilder.Append(PathData)
                .Append(SctiptType).Append(".")
                .Append(SctiptName).Append(".")
                .Append(SctiptName).Append(".")
                .Append(VersionStr).Append(".")
                .Append(ExtensionStr);

            return sbToStringBuilder.ToString();
        }

        public static bool operator >(MigrationScriptResourceData a, MigrationScriptResourceData b)
        {
            int versionA = int.MinValue;
            int.TryParse(a.VersionStr.Substring(1), out versionA);

            int versionB = int.MinValue;
            int.TryParse(b.VersionStr.Substring(1), out versionB);

            if (versionA == versionB)
                return a.VersionStr.CompareTo(b.VersionStr) > 0;

            return versionA > versionB;
        }

        public static bool operator <(MigrationScriptResourceData a, MigrationScriptResourceData b)
        {
            int versionA = int.MinValue;
            int.TryParse(a.VersionStr.Substring(1), out versionA);

            int versionB = int.MinValue;
            int.TryParse(b.VersionStr.Substring(1), out versionB);

            if (versionA == versionB)
                return a.VersionStr.CompareTo(b.VersionStr) < 0;

            return versionA < versionB;
        }
    }
}
