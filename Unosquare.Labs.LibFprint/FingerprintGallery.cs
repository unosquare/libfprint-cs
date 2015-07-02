namespace Unosquare.Labs.LibFprint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides means to keeping a fingerprint database.
    /// A fingerprint gallery is needed for fingerprint identification.
    /// Fingerprints can be loaded from byte arrays or enrollment results.
    /// </summary>
    public class FingerprintGallery : IDisposable
    {
        #region Support Classes

        /// <summary>
        /// Tuple holding Identifier-Pointer pairs.
        /// Keys are strings, while values are pointers to fingerprint data
        /// </summary>
        private class Fingerprint
        {
            public IntPtr Reference { get; set; }
            public string Identifier { get; set; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// This is an ordered list of Fingerprint tuples (key-pointer pairs)
        /// This is basically an in-memory database.
        /// </summary>
        private readonly List<Fingerprint> InternalList = new List<Fingerprint>();

        /// <summary>
        /// An ordered array of pointers to match an index when identification is required.
        /// Whenever the internal list changes, this array gets rebuilt.
        /// </summary>
        /// <value>
        /// The pointer array.
        /// </value>
        internal IntPtr[] PointerArray { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FingerprintGallery"/> class.
        /// </summary>
        public FingerprintGallery()
            : base()
        {
            if (FingerprintDeviceManager.Instance.IsInitialized == false)
                FingerprintDeviceManager.Instance.Initialize();

            this.PointerArray = new IntPtr[0];
        }

        /// <summary>
        /// Rebuilds the pointer array.
        /// This method needs to be called whenever the Internal List gets modified in any way.
        /// </summary>
        private void RebuildPointerArray()
        {
            this.PointerArray = InternalList.Select(s => s.Reference).ToArray();
        }

        /// <summary>
        /// Registers the supplied fingerprint data, associating it with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="fingerprintData">The fingerprint data.</param>
        /// <exception cref="ArgumentException">
        /// fingerprintData is invalid
        /// or
        /// key needs to contain a valid string
        /// </exception>
        /// <exception cref="FormatException">The fingerprint data buffer is invalid.</exception>
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

            if (this.HasKey(key))
                this.Remove(key, false);

            InternalList.Add(new Fingerprint() { Identifier = key, Reference = printDataFromBufferPtr });
        }

        /// <summary>
        /// Adds the specified fingerprint data and associates it with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="fingerprintData">The fingerprint data.</param>
        public void Add(string key, byte[] fingerprintData)
        {
            this.RegisterFingerprintData(key, fingerprintData);
            RebuildPointerArray();
        }

        /// <summary>
        /// Adds the specified fingerprint data and associates it with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="enrollResult">The enroll result.</param>
        public void Add(string key, EnrollStageResult enrollResult)
        {
            this.Add(key, enrollResult.FingerprintData);
        }

        /// <summary>
        /// Adds the specified fingerprint key-value pairs to the gallery.
        /// This is the preferred method for bulk loading as it does not rebuild the database
        /// for every fingerprint.
        /// </summary>
        /// <param name="fingerprints">The fingerprints.</param>
        public void AddRange(IEnumerable<Tuple<string, byte[]>> fingerprints)
        {
            foreach (var fingerprintTuple in fingerprints)
            {
                RegisterFingerprintData(fingerprintTuple.Item1, fingerprintTuple.Item2);
            }

            RebuildPointerArray();
        }

        /// <summary>
        /// Gets the key with the specified offset.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public string this[int offset]
        {
            get
            {
                return this.InternalList[offset].Identifier;
            }
        }

        /// <summary>
        /// Determines whether the gallery contains the specified keys.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return this.AllKeys().Contains(key);
        }

        /// <summary>
        /// Gets all the keys registered in this gallery.
        /// </summary>
        /// <returns></returns>
        public string[] AllKeys()
        {
            return this.InternalList.Select(s => s.Identifier).ToArray();
        }

        /// <summary>
        /// Removes a fingerprint from the gallery given its key
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            this.Remove(key, true);
        }

        /// <summary>
        /// Removes a fingerprint from the gallery given its key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="rebuild">if set to <c>true</c> rebuilds the pointer array.</param>
        /// <exception cref="KeyNotFoundException">The key identifier was not found.</exception>
        private void Remove(string key, bool rebuild)
        {
            var items = InternalList.Where(f => f.Identifier.Equals(key)).ToArray();
            if (items.Length == 0)
                throw new KeyNotFoundException("The key identifier was not found.");

            InternalList.Remove(items[0]);
            Interop.fp_print_data_free(items[0].Reference);

            if (rebuild)
                RebuildPointerArray();
        }

        /// <summary>
        /// Gets the fingerprint pointer.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal IntPtr GetFingerprintPointer(string key)
        {
            if (this.HasKey(key))
                return InternalList.Where(f => f.Identifier.Equals(key)).FirstOrDefault().Reference;

            return IntPtr.Zero;
        }

        #endregion

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
