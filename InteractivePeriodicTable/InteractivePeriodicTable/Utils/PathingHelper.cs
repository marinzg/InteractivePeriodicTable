using System.IO;

namespace InteractivePeriodicTable
{
    public static class PathingHelper
    {
        public static readonly string localDir = setLocalDir();
        public static readonly string resourcesDir = setResourcesDir(localDir);


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
