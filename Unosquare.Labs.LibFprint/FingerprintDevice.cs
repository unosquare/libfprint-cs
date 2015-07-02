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


        public FingerprintDevice()
        {
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

        // TODO: This needs quite a bit of work...
        public EnrollResult Enroll()
        {
            if (IsOpen == false)
                this.Open();

            var printDataPtr = IntPtr.Zero;

            int enrollResult = 0;

            while (true)
            {
                Console.WriteLine("Press your finger to enroll it");
                var printImagePtr = IntPtr.Zero;
                enrollResult = Interop.fp_enroll_finger_img(this.RealDevicePtr, out printDataPtr, out printImagePtr);

                if (printImagePtr != IntPtr.Zero)
                {
                    Interop.fp_img_save_to_file(printImagePtr, "last_enrolled.pgm");
                    Interop.fp_img_free(printImagePtr);

                    using (var bitmap = PgmFormatReader.Read("last_enrolled.pgm"))
                    {
                        bitmap.Save("last_enrolled.png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                    Console.WriteLine("Image saved :)");
                }

                Console.WriteLine("Enroll Result = " + enrollResult);

                if (enrollResult == (int)Interop.fp_enroll_result.FP_ENROLL_COMPLETE)
                    break;
                else
                    this.Reset();
            }

            // We now have access to the Print Data structure
            var printData = printDataPtr.DereferencePtr<Interop.fp_print_data>();
            Console.WriteLine("We have new print data! Type: {0}, Driver: {1}", printData.type, printData.driver_id);

            // We will now need to convert print data to a standard, raw buffer.
            var bufferPtr = IntPtr.Zero;
            var bufferLength = System.Convert.ToInt32(Interop.fp_print_data_get_data(printDataPtr, out bufferPtr));
            var bufferDataArray = new byte[System.Convert.ToInt32(bufferLength)];
            System.Runtime.InteropServices.Marshal.Copy(bufferPtr, bufferDataArray, 0, bufferDataArray.Length);
            var firstCharCount = 3;
            var firstChars = new byte[firstCharCount];
            for (var i = 0; i < firstCharCount; i++)
            {
                firstChars[i] = bufferDataArray[i];
            }

            Console.WriteLine("The standard data buffer has {0} bytes and Reads as follows: {1} (...) ", bufferDataArray.Length, System.Text.Encoding.ASCII.GetString(firstChars));
            System.IO.File.WriteAllBytes("sample.print", bufferDataArray);
            Console.WriteLine("Raw print data was saved");

            // Now let's test how loading of the raw data bufer works
            var printDataFromBufferPtr = Interop.fp_print_data_from_data(bufferDataArray, System.Convert.ToUInt32(bufferDataArray.Length));
            var printDataFromBuffer = printDataFromBufferPtr.DereferencePtr<Interop.fp_print_data>();
            Console.WriteLine("We have print data from Buffer -- Type: {0}, Driver: {1}", printDataFromBuffer.type, printDataFromBuffer.driver_id);

            // We are the muscal genius of this generation at this point
            // Now, let's create gallery with a print and try to identify it (not very challenging, or is it???)
            // The documentation says print gallery is:
            //     NULL-terminated array of pointers to the prints to identify against. 
            //     Each one must have been previously enrolled with a device compatible to the device selected to perform the scan

            var printGalleryPtrList = new System.Collections.Generic.List<IntPtr>();
            printGalleryPtrList.Add(printDataFromBufferPtr);
            var printGalleryArray = printGalleryPtrList.ToArray();

            Console.WriteLine("Place your finger on the scanner again.");
            uint matchOffset = 9999;
            var matchResult = Interop.fp_identify_finger_img(this.RealDevicePtr, printGalleryArray, ref matchOffset, IntPtr.Zero);

            if (matchResult >= 0)
            {
                Console.WriteLine("Verify result: " + ((Interop.fp_verify_result)matchResult).ToString());
                Console.WriteLine("Match offset resulted in: " + matchOffset);
            }

            // -- We need the free method to clear the bufferPtr allocated by the lib previously...
            Interop.fp_print_data_free(printDataPtr);

            foreach (var galleryPrintPtr in printGalleryPtrList)
            {
                if (galleryPrintPtr == IntPtr.Zero)
                    continue;
                else
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(galleryPrintPtr);
            }


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
