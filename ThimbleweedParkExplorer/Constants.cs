using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThimbleweedParkExplorer
{
    public static class Constants
    {
        // Change the version in Properties/AssemblyInfo.cs 
        public static string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        public const string URL = "http://quickandeasysoftware.net";
        public const string ProgName = "Dinky Explorer";
        public const string AllFiles_ContextMenu = "All Files";
    }
}
