using System;

namespace Unosquare.Labs.LibFprint
{
    static internal class Extensions
    {
        static public T DereferencePtr<T>(this IntPtr ptr)
            where T : struct
        {
            return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(T));
        }

        static public T DereferenceDoublePtr<T>(this IntPtr ptr)
            where T : struct
        {
            var secondPtr = ptr.DereferencePtr<IntPtr>();
            return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(secondPtr, typeof(T));
        }
    }
}

