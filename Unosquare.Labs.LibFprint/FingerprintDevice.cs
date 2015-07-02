namespace Unosquare.Labs.LibFprint
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides properties and methods to control and access fingerprint device finctionality.
    /// Use the FingerPrintDeviceManager class to discover and open instances of this class.
    /// Please note this class makes use of unmanaged resources. Dispose accordingly.
    /// </summary>
    public class FingerprintDevice : IDisposable
    {
        #region Properties

        internal Nullable<Interop.fp_dscv_dev> DiscoveredDevice { get; set; }
        internal IntPtr DiscoveredDevicePtr { get; set; }
        internal Nullable<Interop.fp_driver> Driver { get; set; }
        internal IntPtr RealDevicePtr { get; set; }
        internal Nullable<Interop.fp_dev> RealDevice { get; set; }

        /// <summary>
        /// Gets a value indicating whether this device is open and ready for operation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get { return RealDevice.HasValue; } }
        /// <summary>
        /// Gets or sets a value indicating whether this device supports imaging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports imaging]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsImaging { get; protected set; }
        /// <summary>
        /// Gets or sets a value indicating whether this device supports identification.
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports identification]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsIdentification { get; protected set; }
        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; protected set; }
        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; protected set; }
        /// <summary>
        /// Gets or sets a value indicating whether this device supports variable imaging dimensions.
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports variable imaging dimensions]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsVariableImagingDimensions { get; protected set; }
        /// <summary>
        /// Gets the enroll stages count for this device.
        /// </summary>
        /// <value>
        /// The enroll stages count.
        /// </value>
        public int EnrollStagesCount { get; protected set; }
        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        /// <value>
        /// The name of the driver.
        /// </value>
        public string DriverName { get { return Driver.HasValue ? Driver.Value.name : null; } }
        /// <summary>
        /// Gets the full name of the driver.
        /// </summary>
        /// <value>
        /// The full name of the driver.
        /// </value>
        public string DriverFullName { get { return Driver.HasValue ? Driver.Value.full_name : null; } }

        #endregion

        #region Basic functions

        /// <summary>
        /// Prevents ourside instantiation of the <see cref="FingerprintDevice"/> class.
        /// </summary>
        internal FingerprintDevice()
        {
            // We want to prevent standalone instances of this class.
            // Only instances created via FingerprintDeviceManager are valid.
        }

        /// <summary>
        /// Helper method to save and free scanned images
        /// </summary>
        /// <param name="printImagePtr">The print image PTR.</param>
        /// <param name="pgmFilePath">The PGM file path.</param>
        /// <param name="freeImage">if set to <c>true</c> [free image].</param>
        private static void SaveImageToDisk(IntPtr printImagePtr, string pgmFilePath, bool freeImage)
        {
            // Save the PGM file if required by the user
            if (string.IsNullOrWhiteSpace(pgmFilePath) == false && printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_save_to_file(printImagePtr, pgmFilePath);
            }

            // Free the image pointer immediately
            if (freeImage && printImagePtr != IntPtr.Zero)
            {
                Interop.fp_img_free(printImagePtr);
            }
        }

        /// <summary>
        /// Opens this fingerprint scanning device.
        /// This method has to be called before operating the device
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

        #endregion

        #region Enrollment

        /// <summary>
        /// Enrolls the fingerprint.
        /// </summary>
        /// <param name="pgmFilePath">The PGM file path.</param>
        /// <returns></returns>
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
            SaveImageToDisk(printImagePtr, pgmFilePath, true);

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

        /// <summary>
        /// Enrolls the fingerprint.
        /// </summary>
        /// <returns></returns>
        public EnrollStageResult EnrollFingerprint()
        {
            return EnrollFingerprint(null);
        }

        #endregion

        #region Verification

        /// <summary>
        /// Verifies the fingerprint.
        /// </summary>
        /// <param name="galleryKey">The gallery key.</param>
        /// <param name="gallery">The gallery.</param>
        /// <param name="pgmFilePath">The PGM file path.</param>
        /// <returns></returns>
        public bool VerifyFingerprint(string galleryKey, FingerprintGallery gallery, string pgmFilePath)
        {
            // Make sure the device is open
            if (IsOpen == false)
                this.Open();

            // Acquire the pointer to the stored fingerprint
            var fingerprintPtr = gallery.GetFingerprintPointer(galleryKey);
            if (fingerprintPtr == IntPtr.Zero) return false;

            var printImagePtr = IntPtr.Zero;

            // Save the PGM file if required by the user
            SaveImageToDisk(printImagePtr, pgmFilePath, true);

            // Perform verification
            var resultCode = Interop.fp_verify_finger_img(this.RealDevicePtr, fingerprintPtr, out printImagePtr);
            if (resultCode == (int)Interop.fp_verify_result.FP_VERIFY_MATCH)
                return true;

            return false;
        }

        /// <summary>
        /// Verifies the fingerprint.
        /// </summary>
        /// <param name="galleryKey">The gallery key.</param>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public bool VerifyFingerprint(string galleryKey, FingerprintGallery gallery)
        {
            return this.VerifyFingerprint(galleryKey, gallery, null);
        }

        /// <summary>
        /// Verifies the fingerprint.
        /// </summary>
        /// <param name="enrollResult">The enroll result.</param>
        /// <param name="pgmFilePath">The PGM file path.</param>
        /// <returns></returns>
        public bool VerifyFingerprint(EnrollStageResult enrollResult, string pgmFilePath)
        {
            const string galleryKey = "dummy";
            using (var gallery = new FingerprintGallery())
            {
                gallery.Add(galleryKey, enrollResult);
                return VerifyFingerprint(galleryKey, gallery, pgmFilePath);
            }
        }

        /// <summary>
        /// Verifies the fingerprint.
        /// </summary>
        /// <param name="enrollResult">The enroll result.</param>
        /// <returns></returns>
        public bool VerifyFingerprint(EnrollStageResult enrollResult)
        {
            return this.VerifyFingerprint(enrollResult, null);
        }

        #endregion

        #region Identification

        /// <summary>
        /// Identifies the fingerprint.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="pgmFilePath">The PGM file path.</param>
        /// <returns></returns>
        public string IdentifyFingerprint(FingerprintGallery gallery, string pgmFilePath)
        {
            // Make sure the device is open
            if (IsOpen == false)
                this.Open();

            uint matchOffset = 0;
            var printImagePtr = IntPtr.Zero;

            var matchResult = Interop.fp_identify_finger_img(this.RealDevicePtr, gallery.PointerArray, out matchOffset, out printImagePtr);

            // Save the PGM file if required by the user
            SaveImageToDisk(printImagePtr, pgmFilePath, true);

            // Return the key string based on the offset
            if (matchResult == 1)
            {
                return gallery[Convert.ToInt32(matchOffset)];
            }

            return null;

        }

        /// <summary>
        /// Identifies the fingerprint.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        public string IdentifyFingerprint(FingerprintGallery gallery)
        {
            return IdentifyFingerprint(gallery, null);
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
