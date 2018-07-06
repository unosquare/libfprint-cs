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
        private static FingerprintDeviceManager m_Instance;

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
        public static FingerprintDeviceManager Instance => m_Instance ?? (m_Instance = new FingerprintDeviceManager());

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
        public void Initialize(int debugLevel = 0)
        {
            if (IsInitialized) return;

            var result = Interop.fp_init();
            if (result < 0)
                throw new InvalidOperationException($"Failed to initialize fprint library. Result code: {result}");

            Interop.fp_set_debug(debugLevel);

            IsInitialized = true;
        }

        /// <summary>
        /// Performs a discovery of currently connected Fingerprint Scanner devices
        /// Use the resulting array of FingerPrintDevice to start operating the devices.
        /// </summary>
        /// <returns></returns>
        public FingerprintDevice[] DiscoverDevices()
        {
            if (!IsInitialized) Initialize();

            var result = new List<FingerprintDevice>();

            ReleaseDeviceDiscoveryResults();
            var dicoveredDevicePtrs = Interop.fp_discover_devs_pointers(out var deviceDiscoveryResultsPtr); // This returns the array of pointrs
            DeviceDicoveryResultsPtr = deviceDiscoveryResultsPtr;

            foreach (var devicePtr in dicoveredDevicePtrs)
            {
                var deviceStruct = devicePtr.DereferencePtr<Interop.fp_dscv_dev>();
                var driverStruct = Interop.fp_dscv_dev_get_driver_struct(ref deviceStruct);

                var managedDevice = new FingerprintDevice
                {
                    DiscoveredDevice = deviceStruct,
                    DiscoveredDevicePtr = devicePtr,
                    Driver = driverStruct
                };

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

        /// <inheritdoc />
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
