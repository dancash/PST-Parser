using System;
using System.Collections.Generic;
using System.Threading;

namespace PSTParse.Utilities
{
    public static class Utilities
    {
        public static T Lazy<T>(ref T field, Func<T> func) where T : class
        {
            return LazyInitializer.EnsureInitialized(ref field, func);
        }

        //public static T Lazy2<T>(LazyStruct<T> lazyStruct, Func<LazyStruct<T>> func) where T : struct
        //{
        //    return LazyInitializer.EnsureInitialized(ref lazyStruct, func, ref);
        //}

        //public static T Lazy2<T>(LazyStruct<T> lazyStruct, Func<LazyStruct<T>> func) where T : struct
        //{


        //    throw new NotImplementedException();
        //}
    }

    //public static class LazyStruct
    //{
    //    public static LazyStruct<T> Create<T>(Func<T> func) where T : struct => new LazyStruct<T>();
    //    public static LazyStruct<T> Default<T>() where T : struct => new LazyStruct<T>();
    //}

    //public struct LazyStruct<T> where T : struct
    //{
    //    private readonly Func<T> _func;
    //    private T _value;

    //    public bool HasValue { get; }
    //    public T Value
    //    {
    //        get
    //        {
    //            if (HasValue) return _value;
    //            _value = _func();
    //            return _value;
    //        }
    //    }

    //    public LazyStruct(Func<T> func)
    //    {
    //        _func = func;
    //        _value = default;
    //        HasValue = false;
    //    }
    //}
}
