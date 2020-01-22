using System.Collections.Generic;
using System.Linq;

namespace SaaS.WinService.Core
{
    public static class Enums
    {
        public static IEnumerable<T> Get<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}