namespace Unosquare.Labs.LibFprint
{
    using System;

    /// <summary>
    /// Helper methods for pointer management.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Dereferences the PTR into the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr">The PTR.</param>
        /// <returns></returns>
        public static T DereferencePtr<T>(this IntPtr ptr) => (T)System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(T));

        /// <summary>
        /// Turns the given base address to an array of pointers.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <returns></returns>
        public static IntPtr[] ToPointerArray(this IntPtr baseAddress)
        {
            var ptrList = new System.Collections.Generic.List<IntPtr>();

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
    }
}