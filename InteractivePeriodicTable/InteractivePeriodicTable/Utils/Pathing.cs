using System.IO;

namespace InteractivePeriodicTable
{
    public static class Pathing
    {
        public static readonly string localDir = setLocalDir();
        public static readonly string resourcesDir = setResourcesDir(localDir);
        public static readonly string sysDir = resourcesDir+"\\Sys\\";
        public static readonly string imgDir = resourcesDir + "\\Images\\Quiz\\";


        private static string setResourcesDir(string path)
        {
            path = path + "\\Resources";
            return path;
        }

        private static string setLocalDir()
        {
            string tmp = Directory.GetCurrentDirectory();
            tmp = tmp.Remove(tmp.IndexOf("\\bin"));
            return tmp;
        }
    }
}
