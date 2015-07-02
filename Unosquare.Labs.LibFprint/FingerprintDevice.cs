using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
    public class FingerprintDevice : IDisposable
    {
        internal Nullable<Interop.fp_dscv_dev> DiscoveredDevice { get; set; }
        internal IntPtr DiscoveredDevicePtr { get; set; }
        internal Nullable<Interop.fp_driver> Driver { get; set; }

        internal IntPtr RealDevicePtr { get; set; }
        internal Nullable<Interop.fp_dev> RealDevice { get; set; }


        internal FingerprintDevice()
        {
            // We want to prevent standalone instances of this class.
            // Only instances created via FingerprintDeviceManager are valid.
        }

        /// <summary>
        /// Opens this fingerprint scanning device.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Could not open device.</exception>
        public void Open()
        {
            this.RealDevicePtr = Interop.fp_dev_open(DiscoveredDevicePtr);
            if (this.RealDevicePtr == IntPtr.Zero)
                throw new InvalidOperationException("Could not open device.");

            this.RealDevice = RealDevicePtr.DereferencePtr<Interop.fp_dev>();

            // Populate device info upon opening
            var realDevice = this.RealDevice.Value;
            this.SupportsImaging = Interop.fp_dev_supports_imaging(ref realDevice) != 0;
            this.SupportsIdentification = Interop.fp_dev_supports_identification(ref realDevice) != 0;
            this.ImageHeight = Interop.fp_dev_get_img_height(ref realDevice);
            this.ImageWidth = Interop.fp_dev_get_img_width(ref realDevice);
            this.SupportsVariableImagingDimensions = this.ImageWidth <= 0 || this.ImageHeight <= 0;
            this.EnrollStagesCount = Interop.fp_dev_get_nr_enroll_stages(ref realDevice);
        }

        /// <summary>
        /// Resets the device by closing and re-opening it.
        /// </summary>
        public void Reset()
        {
            if (IsOpen == false) return;
            Interop.fp_dev_close(this.RealDevicePtr);
            this.Open();
        }

        public EnrollStageResult EnrollFingerprint()
        {
            return EnrollFingerprint(null);
        }

        public bool VerifyFingerprint(string pgmFilePath)
        {
            // TODO: Implement.
            return false;
        }

        public EnrollStageResult EnrollFingerprint(string pgmFilePath)
        {
            // Make sure the device is open
            if (IsOpen == false)
                this.Open();

            var enrollResultCode = 0;
            var printDataPtr = IntPtr.Zero;
            var printImagePtr = IntPtr.Zero;
            byte[] fingerprintData = null;

            enrollResultCode = Interop.fp_enroll_finger_img(this.RealDevicePtr, out printDataPtr, out printImagePtr);

            // Save the PGM file if required by the user
            if (string.IsNullOrWhiteSpace(pgmFilePath) == false && printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_save_to_file(printImagePtr, pgmFilePath);
            }

            // Free the image pointer immediately
            if (printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_free(printImagePtr);
            }

            // Create the fingerprint data buffer if the enroll fp data is available
            if (enrollResultCode == (int)EnrollResult.EnrollStagePassed || enrollResultCode == (int)EnrollResult.EnrollComplete)
            {
                if (printDataPtr != IntPtr.Zero)
                {
                    var bufferPtr = IntPtr.Zero;
                    var bufferLength = System.Convert.ToInt32(Interop.fp_print_data_get_data(printDataPtr, out bufferPtr));
                    fingerprintData = new byte[bufferLength];
                    Marshal.Copy(bufferPtr, fingerprintData, 0, fingerprintData.Length);
                    Interop.fp_print_data_free(printDataPtr);
                    // TODO: free(bufferPtr) // Maybe Marshal.FreeHGlobal as done in the following line?
                    Marshal.FreeHGlobal(bufferPtr);
                }
            }

            return new EnrollStageResult(enrollResultCode, fingerprintData);
        }

        public string IdentifyFingerprint(FingerprintGallery gallery)
        {
            return IdentifyFingerprint(gallery, null);
        }

        public string IdentifyFingerprint(FingerprintGallery gallery, string pgmFilePath)
        {
            // Make sure the device is open
            if (IsOpen == false)
                this.Open();

            uint matchOffset = 0;
            var printImagePtr = IntPtr.Zero;

            var matchResult = Interop.fp_identify_finger_img(this.RealDevicePtr, gallery.PointerArray, out matchOffset, out printImagePtr);

            // Save the PGM file if required by the user
            if (string.IsNullOrWhiteSpace(pgmFilePath) == false && printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_save_to_file(printImagePtr, pgmFilePath);
            }

            // Free the image pointer immediately
            if (printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_free(printImagePtr);
            }

            // Return the key string based on the offset
            if (matchResult == 1)
            {
                return gallery[Convert.ToInt32(matchOffset)];
            }

            return null;

        }

        public bool IsOpen { get { return RealDevice.HasValue; } }

        public bool SupportsImaging { get; protected set; }
        public bool SupportsIdentification { get; protected set; }
        public int ImageHeight { get; protected set; }
        public int ImageWidth { get; protected set; }
        public bool SupportsVariableImagingDimensions { get; protected set; }
        public int EnrollStagesCount { get; protected set; }

        public string DriverName { get { return Driver.HasValue ? Driver.Value.name : null; } }

        public string DriverFullName { get { return Driver.HasValue ? Driver.Value.full_name : null; } }


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
        ~FingerprintDevice()
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
            if (IsOpen)
            {
                Interop.fp_dev_close(this.RealDevicePtr);
                this.RealDevice = null;
                this.RealDevicePtr = IntPtr.Zero;
            }
        }

        #endregion
    }
}
