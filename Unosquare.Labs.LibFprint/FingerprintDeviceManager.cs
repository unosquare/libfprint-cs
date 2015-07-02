namespace Unosquare.Labs.LibFprint
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Singleton class to Initialize the fprint library and perform fingerprint reader device discovery.
    /// Access properties and methods via the Instance Property.
    /// </summary>
    public sealed class FingerprintDeviceManager : IDisposable
    {

        #region Singleton Implementation

        /// <summary>
        /// The static, singleton instance reference.
        /// </summary>
        private static FingerprintDeviceManager m_Instance = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="FingerprintDeviceManager"/> class from being created.
        /// </summary>
        private FingerprintDeviceManager()
        {
            // placeholder
        }

        /// <summary>
        /// Gets the single instance of the Fingerprint Device Manager
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static FingerprintDeviceManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new FingerprintDeviceManager();

                return m_Instance;
            }
            private set
            {
                m_Instance = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the library is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }


        /// <summary>
        /// Holds a pointer to the base address of the array containing
        /// Discovered device references.
        /// </summary>
        /// <value>
        /// The device dicovery results PTR.
        /// </value>
        private IntPtr DeviceDicoveryResultsPtr { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the fprint library with the given debug level.
        /// The dibug level is not documented but 3 seems to have some level of verbosity.
        /// </summary>
        /// <param name="debugLevel">The debug level.</param>
        /// <exception cref="System.Exception"></exception>
        public void Initialize(int debugLevel)
        {
            if (IsInitialized) return;

            var result = Interop.fp_init();
            if (result < 0)
                throw new Exception(string.Format("Failed to initialize fprint library. Result code: {0}", result));

            Interop.fp_set_debug(debugLevel);

            this.IsInitialized = true;
        }

        /// <summary>
        /// Initializes the fprint library with Debug Level 0.
        /// </summary>
        public void Initialize()
        {
            this.Initialize(0);
        }

        /// <summary>
        /// Performs a discovery of currently connected Fingerprint Scanner devices
        /// Use the resulting array of FingerPrintDevice to start operating the devices.
        /// </summary>
        /// <returns></returns>
        public FingerprintDevice[] DiscoverDevices()
        {
            if (!IsInitialized) this.Initialize();

            var result = new List<FingerprintDevice>();

            this.ReleaseDeviceDiscoveryResults();
            var deviceDiscoveryResultsPtr = IntPtr.Zero; // This will point to an array of pointers
            var dicoveredDevicePtrs = Interop.fp_discover_devs_pointers(out deviceDiscoveryResultsPtr); // This returns the array of pointrs
            this.DeviceDicoveryResultsPtr = deviceDiscoveryResultsPtr;

            foreach (var devicePtr in dicoveredDevicePtrs)
            {
                var deviceStruct = devicePtr.DereferencePtr<Interop.fp_dscv_dev>();
                var driverStruct = Interop.fp_dscv_dev_get_driver_struct(ref deviceStruct);

                var managedDevice = new FingerprintDevice();
                managedDevice.DiscoveredDevice = deviceStruct;
                managedDevice.DiscoveredDevicePtr = devicePtr;
                managedDevice.Driver = driverStruct;

                result.Add(managedDevice);
            }

            return result.ToArray();

        }

        #endregion

        #region IDisposable Implementation and cleanup

        /// <summary>
        /// Releases the device discovery results.
        /// This is only for internal purposes.
        /// </summary>
        private void ReleaseDeviceDiscoveryResults()
        {
            if (DeviceDicoveryResultsPtr != IntPtr.Zero)
            {
                // Destroy the memory allocation that resulted from device discovery
                Interop.fp_dscv_devs_free(DeviceDicoveryResultsPtr);
                DeviceDicoveryResultsPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FingerprintDeviceManager"/> class.
        /// </summary>
        ~FingerprintDeviceManager()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="releaseOnlyManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool releaseOnlyManaged)
        {
            if (releaseOnlyManaged)
            {
                return;
            }

            // free native resources if there are any.
            if (IsInitialized)
            {
                ReleaseDeviceDiscoveryResults();
                Interop.fp_exit();
                IsInitialized = false;
            }

            // remove the reference to the singleton so it can be recreated.
            m_Instance = null;
        }

        #endregion

    }
}
