using System.IO;

namespace InteractivePeriodicTable
{
    public static class PathingHelper
    {
        public static readonly string localDir=Directory.GetCurrentDirectory();
        public static readonly string resourcesDir = setResourcesDir(localDir);


        private static string setResourcesDir(string path)
        {
            path = path.Remove(path.IndexOf("\\bin\\"));
            path = path + "\\Resources";
            return path;
        }
    }
}
