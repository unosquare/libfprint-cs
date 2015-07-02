using System;
using System.Collections.Generic;

namespace Unosquare.Labs.LibFprint
{
    static internal class Extensions
    {
        static public T DereferencePtr<T>(this IntPtr ptr)
        {
            return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(T));
        }

        static public T DereferenceDoublePtr<T>(this IntPtr ptr)
        {
            var secondPtr = ptr.DereferencePtr<IntPtr>();
            return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(secondPtr, typeof(T));
        }


        static public IntPtr[] ToPointerArray(this IntPtr baseAddress)
        {
            var ptrList = new System.Collections.Generic.List<IntPtr>();

            //var arrayPtr = baseAddress;
            if (baseAddress == IntPtr.Zero) return ptrList.ToArray();

            while (true)
            {
                var itemPtr = baseAddress.DereferencePtr<IntPtr>();
                if (itemPtr == IntPtr.Zero)
                    break;

                ptrList.Add(itemPtr);
                baseAddress = IntPtr.Add(baseAddress, IntPtr.Size);
            }

            return ptrList.ToArray();
        }

        static public T[] ToArray<T>(this IntPtr baseAddress)
        {
            var pointerArray = baseAddress.ToPointerArray();

            var resultList = new List<T>();
            foreach (var ptr in pointerArray)
            {
                resultList.Add(ptr.DereferencePtr<T>());
            }

            return resultList.ToArray();
        }
    }
}

