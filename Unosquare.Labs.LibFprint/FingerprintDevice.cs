using System;
using System.Collections.Generic;
using System.Linq;
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

        // TODO: This needs quite a bit of work...
        public EnrollResult Enroll()
        {
            if (IsOpen == false)
                this.Open();

            var printDataPtr = IntPtr.Zero;

            int enrollResult = 0;

            do
            {
                var printImagePtr = IntPtr.Zero;
                enrollResult = Interop.fp_enroll_finger_img(this.RealDevicePtr, out printDataPtr, out printImagePtr);

                if (printImagePtr != IntPtr.Zero)
                {
                    Interop.fp_img_save_to_file(printImagePtr, "enrolled.pgm");
                    Interop.fp_img_free(printImagePtr);
                }

                Console.WriteLine("Enroll Result = " + enrollResult);

            } while (enrollResult != (int)Interop.fp_enroll_result.FP_ENROLL_COMPLETE);

            var finalResult = (EnrollResult)enrollResult;
            return finalResult;
        }

        public void Identify()
        {
            // http://www.reactivated.net/fprint/api/
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
