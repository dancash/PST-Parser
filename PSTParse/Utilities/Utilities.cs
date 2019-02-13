using System;
using System.Threading;

namespace PSTParse.Utilities
{
    public static class Utilities
    {
        public static T Lazy<T>(ref T field, Func<T> func) where T : class
        {
            return LazyInitializer.EnsureInitialized(ref field, func);
        }
    }
}
