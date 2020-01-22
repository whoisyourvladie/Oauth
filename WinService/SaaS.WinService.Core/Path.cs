using System.IO;
using System.Reflection;

namespace SaaS.WinService.Core
{
    public static class Path
    {
        public static DirectoryInfo CurrentDirectoryInfo
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return new DirectoryInfo(System.IO.Path.GetDirectoryName(assembly.GetName().CodeBase).Remove(0, "file:\\".Length));
            }
        }
    }
}
