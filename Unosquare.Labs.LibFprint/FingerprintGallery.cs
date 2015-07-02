using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
    public class FingerprintGallery : IDisposable
    {

        private readonly List<Fingerprint> InternalList = new List<Fingerprint>();
        internal IntPtr[] PointerArray { get; private set; }

        private class Fingerprint
        {
            public IntPtr Reference { get; set; }
            public string Identifier { get; set; }
        }

        public FingerprintGallery()
            : base()
        {
            if (FingerprintDeviceManager.Instance.IsInitialized == false)
                FingerprintDeviceManager.Instance.Initialize();

            this.PointerArray = new IntPtr[0];
        }

        private void RebuildPointerArray()
        {
            this.PointerArray = InternalList.Select(s => s.Reference).ToArray();
        }

        private void RegisterFingerprintData(string key, byte[] fingerprintData)
        {
            if (fingerprintData == null || fingerprintData.Length <= 0)
                throw new ArgumentException("fingerprintData is invalid");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key needs to contain a valid string");

            var bufferLength = System.Convert.ToUInt32(fingerprintData.Length);
            var printDataFromBufferPtr = Interop.fp_print_data_from_data(fingerprintData, bufferLength);
            if (printDataFromBufferPtr == IntPtr.Zero)
                throw new FormatException("The fingerprint data buffer is invalid.");

            InternalList.Add(new Fingerprint() { Identifier = key, Reference = printDataFromBufferPtr });
        }

        public void Add(string key, EnrollStageResult enrollResult)
        {
            this.Add(key, enrollResult.FingerprintData);
        }

        public void Add(string key, byte[] fingerprintData)
        {
            this.RegisterFingerprintData(key, fingerprintData);
            RebuildPointerArray();
        }

        public void AddRange(IEnumerable<Tuple<string, byte[]>> fingerprints)
        {
            foreach (var fingerprintTuple in fingerprints)
            {
                RegisterFingerprintData(fingerprintTuple.Item1, fingerprintTuple.Item2);
            }

            RebuildPointerArray();
        }

        public string this[int offset]
        {
            get
            {
                return this.InternalList[offset].Identifier;
            }
        }

        public void Remove(string key)
        {
            var items = InternalList.Where(f => f.Identifier.Equals(key)).ToArray();
            if (items.Length == 0)
                throw new KeyNotFoundException("The key identifier was not found.");

            InternalList.Remove(items[0]);
            Interop.fp_print_data_free(items[0].Reference);

            RebuildPointerArray();
        }

        #region IDisposable Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FingerprintGallery()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="releaseOnlyManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool releaseOnlyManaged)
        {
            if (releaseOnlyManaged)
            {
                return;
            }

            // free native resources if there are any.
            foreach (var fingerprintPtr in PointerArray)
            {
                Interop.fp_print_data_free(fingerprintPtr);
            }
        }

        #endregion
    }
}
