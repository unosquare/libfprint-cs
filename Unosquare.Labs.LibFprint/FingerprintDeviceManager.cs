using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
    public class FingerprintDeviceManager : IDisposable
    {

        #region Singleton Implementation

        /// <summary>
        /// The static, singleton instance reference.
        /// </summary>
        private static FingerprintDeviceManager m_Instance = null;

        protected FingerprintDeviceManager()
        {
            // placeholder
        }

        public static FingerprintDeviceManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new FingerprintDeviceManager();

                return m_Instance;
            }
            protected set
            {
                m_Instance = value;
            }
        }

        #endregion

        public bool IsInitialized { get; protected set; }

        private IntPtr DeviceDicoveryResultsPtr { get; set; }


        /// <summary>
        /// Initializes the fprint library.
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
        /// Initializes the fprint library with Debug Level = 3
        /// </summary>
        public void Initialize()
        {
            this.Initialize(3);
        }

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

        private void ReleaseDeviceDiscoveryResults()
        {
            if (DeviceDicoveryResultsPtr != IntPtr.Zero)
            {
                // Destroy the memory allocation that resulted from device discovery
                Interop.fp_dscv_devs_free(DeviceDicoveryResultsPtr);
                DeviceDicoveryResultsPtr = IntPtr.Zero;
            }
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
        protected virtual void Dispose(bool releaseOnlyManaged)
        {
            if (releaseOnlyManaged)
            {
                // free managed resources
                //if (managedResource != null)
                //{
                //    managedResource.Dispose();
                //    managedResource = null;
                //}

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
