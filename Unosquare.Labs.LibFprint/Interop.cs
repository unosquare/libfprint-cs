using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
    internal partial class Interop
    {
        private const string FingerprintLibrary = "libfprint.so";

        /// Return Type: char*
        ///drv: fp_driver*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_driver_get_name")]
        public static extern System.IntPtr fp_driver_get_name(ref fp_driver drv);


        /// Return Type: char*
        ///drv: fp_driver*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_driver_get_full_name")]
        public static extern System.IntPtr fp_driver_get_full_name(ref fp_driver drv);


        /// Return Type: uint16_t->unsigned short
        ///drv: fp_driver*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_driver_get_driver_id")]
        public static extern ushort fp_driver_get_driver_id(ref fp_driver drv);


        /// Return Type: fp_scan_type
        ///drv: fp_driver*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_driver_get_scan_type")]
        public static extern fp_scan_type fp_driver_get_scan_type(ref fp_driver drv);


        /// Return Type: fp_dscv_dev**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_discover_devs")]
        public static extern IntPtr fp_discover_devs();

        public static IntPtr[] fp_discover_devs_pointers(out IntPtr arrayPtr)
        {

            var baseAddress = fp_discover_devs();
            arrayPtr = baseAddress;

            return baseAddress.ToPointerArray();
        }

        public static fp_dscv_dev[] fp_discover_devs_structus(out IntPtr arrayPtr)
        {
            var devices = new System.Collections.Generic.List<fp_dscv_dev>();
            var devicePtrs = fp_discover_devs_pointers(out arrayPtr);

            foreach (var devicePtr in devicePtrs)
            {
                var device = devicePtr.DereferencePtr<fp_dscv_dev>();
                devices.Add(device);
            }

            return devices.ToArray();
        }

        /// Return Type: void
        ///devs: fp_dscv_dev**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_devs_free")]
        public static extern void fp_dscv_devs_free(IntPtr devs);


        /// Return Type: fp_driver*
        ///dev: fp_dscv_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_get_driver")]
        public static extern IntPtr fp_dscv_dev_get_driver(ref fp_dscv_dev dev);

        public static fp_driver fp_dscv_dev_get_driver_struct(ref fp_dscv_dev dev)
        {
            var ptr = fp_dscv_dev_get_driver(ref dev);
            return ptr.DereferencePtr<fp_driver>();
        }

        /// Return Type: uint32_t->unsigned int
        ///dev: fp_dscv_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_get_devtype")]
        public static extern uint fp_dscv_dev_get_devtype(ref fp_dscv_dev dev);


        /// Return Type: int
        ///dev: fp_dscv_dev*
        ///print: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_supports_print_data")]
        public static extern int fp_dscv_dev_supports_print_data(ref fp_dscv_dev dev, ref fp_print_data print);


        /// Return Type: int
        ///dev: fp_dscv_dev*
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_supports_dscv_print")]
        public static extern int fp_dscv_dev_supports_dscv_print(ref fp_dscv_dev dev, ref fp_dscv_print print);


        /// Return Type: fp_dscv_dev*
        ///devs: fp_dscv_dev**
        ///print: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_for_print_data")]
        public static extern System.IntPtr fp_dscv_dev_for_print_data(ref System.IntPtr devs, ref fp_print_data print);


        /// Return Type: fp_dscv_dev*
        ///devs: fp_dscv_dev**
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_for_dscv_print")]
        public static extern System.IntPtr fp_dscv_dev_for_dscv_print(ref System.IntPtr devs, ref fp_dscv_print print);


        /// Return Type: uint16_t->unsigned short
        ///dev: fp_dscv_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_dev_get_driver_id")]
        public static extern ushort fp_dscv_dev_get_driver_id(ref fp_dscv_dev dev);


        /// Return Type: fp_dscv_print**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_discover_prints")]
        public static extern System.IntPtr fp_discover_prints();


        /// Return Type: void
        ///prints: fp_dscv_print**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_prints_free")]
        public static extern void fp_dscv_prints_free(ref System.IntPtr prints);


        /// Return Type: uint16_t->unsigned short
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_print_get_driver_id")]
        public static extern ushort fp_dscv_print_get_driver_id(ref fp_dscv_print print);


        /// Return Type: uint32_t->unsigned int
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_print_get_devtype")]
        public static extern uint fp_dscv_print_get_devtype(ref fp_dscv_print print);


        /// Return Type: fp_finger
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_print_get_finger")]
        public static extern fp_finger fp_dscv_print_get_finger(ref fp_dscv_print print);


        /// Return Type: int
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dscv_print_delete")]
        public static extern int fp_dscv_print_delete(ref fp_dscv_print print);


        /// Return Type: fp_dev*
        ///ddev: fp_dscv_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_open")]
        public static extern IntPtr fp_dev_open(IntPtr ddev);

        public static fp_dev fp_dev_open_struct(IntPtr ddev, out IntPtr devicePtr)
        {
            devicePtr = fp_dev_open(ddev);
            return devicePtr.DereferencePtr<fp_dev>();
        }

        /// Return Type: void
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_close")]
        public static extern void fp_dev_close(IntPtr dev);


        /// Return Type: fp_driver*
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_get_driver")]
        public static extern System.IntPtr fp_dev_get_driver(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_get_nr_enroll_stages")]
        public static extern int fp_dev_get_nr_enroll_stages(ref fp_dev dev);


        /// Return Type: uint32_t->unsigned int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_get_devtype")]
        public static extern uint fp_dev_get_devtype(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        ///data: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_supports_print_data")]
        public static extern int fp_dev_supports_print_data(ref fp_dev dev, ref fp_print_data data);


        /// Return Type: int
        ///dev: fp_dev*
        ///print: fp_dscv_print*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_supports_dscv_print")]
        public static extern int fp_dev_supports_dscv_print(ref fp_dev dev, ref fp_dscv_print print);


        /// Return Type: int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_supports_imaging")]
        public static extern int fp_dev_supports_imaging(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        ///unconditional: int
        ///image: fp_img**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_img_capture")]
        public static extern int fp_dev_img_capture(IntPtr dev, int unconditional, IntPtr image);


        /// Return Type: int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_get_img_width")]
        public static extern int fp_dev_get_img_width(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_get_img_height")]
        public static extern int fp_dev_get_img_height(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        ///print_data: fp_print_data**
        ///img: fp_img**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_enroll_finger_img")]
        public static extern int fp_enroll_finger_img(IntPtr dev, out System.IntPtr print_data, out System.IntPtr img);


        /// Return Type: int
        ///dev: fp_dev*
        ///print_data: fp_print_data**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_enroll_finger")]
        public static extern int fp_enroll_finger(ref fp_dev dev, ref System.IntPtr print_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///enrolled_print: fp_print_data*
        ///img: fp_img**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_verify_finger_img")]
        public static extern int fp_verify_finger_img(ref fp_dev dev, ref fp_print_data enrolled_print, ref System.IntPtr img);


        /// Return Type: int
        ///dev: fp_dev*
        ///enrolled_print: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_verify_finger")]
        public static extern int fp_verify_finger(ref fp_dev dev, ref fp_print_data enrolled_print);


        /// Return Type: int
        ///dev: fp_dev*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_dev_supports_identification")]
        public static extern int fp_dev_supports_identification(ref fp_dev dev);


        /// Return Type: int
        ///dev: fp_dev*
        ///print_gallery: fp_print_data**
        ///match_offset: size_t*
        ///img: fp_img**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_identify_finger_img")]
        public static extern int fp_identify_finger_img(IntPtr dev, IntPtr[] print_gallery, out uint match_offset, out System.IntPtr img);


        /// Return Type: int
        ///dev: fp_dev*
        ///print_gallery: fp_print_data**
        ///match_offset: size_t*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_identify_finger")]
        public static extern int fp_identify_finger(IntPtr dev, ref fp_print_data[] print_gallery, ref uint match_offset);


        /// Return Type: int
        ///dev: fp_dev*
        ///finger: fp_finger
        ///data: fp_print_data**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_load")]
        public static extern int fp_print_data_load(ref fp_dev dev, fp_finger finger, ref System.IntPtr data);


        /// Return Type: int
        ///print: fp_dscv_print*
        ///data: fp_print_data**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_from_dscv_print")]
        public static extern int fp_print_data_from_dscv_print(ref fp_dscv_print print, ref System.IntPtr data);


        /// Return Type: int
        ///data: fp_print_data*
        ///finger: fp_finger
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_save")]
        public static extern int fp_print_data_save(ref fp_print_data data, fp_finger finger);


        /// Return Type: int
        ///dev: fp_dev*
        ///finger: fp_finger
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_delete")]
        public static extern int fp_print_data_delete(ref fp_dev dev, fp_finger finger);


        /// Return Type: void
        ///data: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_free")]
        public static extern void fp_print_data_free(IntPtr data);


        /// Return Type: size_t->unsigned int
        ///data: fp_print_data*
        ///ret: unsigned char**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_get_data")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.SysUInt)]
        public static extern uint fp_print_data_get_data(IntPtr data, out System.IntPtr ret);


        /// Return Type: fp_print_data*
        ///buf: unsigned char*
        ///buflen: size_t->unsigned int
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_from_data")]
        public static extern System.IntPtr fp_print_data_from_data(
              byte[] buf
            ,[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.SysUInt)] uint buflen);


        /// Return Type: uint16_t->unsigned short
        ///data: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_get_driver_id")]
        public static extern ushort fp_print_data_get_driver_id(ref fp_print_data data);


        /// Return Type: uint32_t->unsigned int
        ///data: fp_print_data*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_print_data_get_devtype")]
        public static extern uint fp_print_data_get_devtype(ref fp_print_data data);


        /// Return Type: int
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_get_height")]
        public static extern int fp_img_get_height(ref fp_img img);


        /// Return Type: int
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_get_width")]
        public static extern int fp_img_get_width(ref fp_img img);


        /// Return Type: unsigned char*
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_get_data")]
        public static extern System.IntPtr fp_img_get_data(ref fp_img img);


        /// Return Type: int
        ///img: fp_img*
        ///path: char*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_save_to_file")]
        public static extern int fp_img_save_to_file(IntPtr img, string path);


        /// Return Type: void
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_standardize")]
        public static extern void fp_img_standardize(ref fp_img img);


        /// Return Type: fp_img*
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_binarize")]
        public static extern System.IntPtr fp_img_binarize(ref fp_img img);


        /// Return Type: fp_minutia**
        ///img: fp_img*
        ///nr_minutiae: int*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_get_minutiae")]
        public static extern System.IntPtr fp_img_get_minutiae(ref fp_img img, ref int nr_minutiae);


        /// Return Type: void
        ///img: fp_img*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_img_free")]
        public static extern void fp_img_free(IntPtr img);


        /// Return Type: int
        ///timeout: timeval*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_handle_events_timeout")]
        public static extern int fp_handle_events_timeout(ref timeval timeout);


        /// Return Type: int
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_handle_events")]
        public static extern int fp_handle_events();


        /// Return Type: size_t->unsigned int
        ///pollfds: fp_pollfd**
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_get_pollfds")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.SysUInt)]
        public static extern uint fp_get_pollfds(ref System.IntPtr pollfds);


        /// Return Type: int
        ///tv: timeval*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_get_next_timeout")]
        public static extern int fp_get_next_timeout(ref timeval tv);


        /// Return Type: void
        ///added_cb: fp_pollfd_added_cb
        ///removed_cb: fp_pollfd_removed_cb
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_set_pollfd_notifiers")]
        public static extern void fp_set_pollfd_notifiers(fp_pollfd_added_cb added_cb, fp_pollfd_removed_cb removed_cb);


        /// Return Type: int
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_init")]
        public static extern int fp_init();


        /// Return Type: void
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_exit")]
        public static extern void fp_exit();


        /// Return Type: void
        ///level: int
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_set_debug")]
        public static extern void fp_set_debug(int level);


        /// Return Type: int
        ///ddev: fp_dscv_dev*
        ///callback: fp_dev_open_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_dev_open")]
        public static extern int fp_async_dev_open(ref fp_dscv_dev ddev, fp_dev_open_cb callback, System.IntPtr user_data);


        /// Return Type: void
        ///dev: fp_dev*
        ///callback: fp_dev_close_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_dev_close")]
        public static extern void fp_async_dev_close(ref fp_dev dev, fp_dev_close_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///callback: fp_enroll_stage_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_enroll_start")]
        public static extern int fp_async_enroll_start(ref fp_dev dev, fp_enroll_stage_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///callback: fp_enroll_stop_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_enroll_stop")]
        public static extern int fp_async_enroll_stop(ref fp_dev dev, fp_enroll_stop_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///data: fp_print_data*
        ///callback: fp_verify_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_verify_start")]
        public static extern int fp_async_verify_start(ref fp_dev dev, ref fp_print_data data, fp_verify_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///callback: fp_verify_stop_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_verify_stop")]
        public static extern int fp_async_verify_stop(ref fp_dev dev, fp_verify_stop_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///gallery: fp_print_data**
        ///callback: fp_identify_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_identify_start")]
        public static extern int fp_async_identify_start(ref fp_dev dev, ref System.IntPtr gallery, fp_identify_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///callback: fp_identify_stop_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_identify_stop")]
        public static extern int fp_async_identify_stop(ref fp_dev dev, fp_identify_stop_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///unconditional: int
        ///callback: fp_capture_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_capture_start")]
        public static extern int fp_async_capture_start(ref fp_dev dev, int unconditional, fp_capture_cb callback, System.IntPtr user_data);


        /// Return Type: int
        ///dev: fp_dev*
        ///callback: fp_capture_stop_cb
        ///user_data: void*
        [System.Runtime.InteropServices.DllImportAttribute(FingerprintLibrary, EntryPoint = "fp_async_capture_stop")]
        public static extern int fp_async_capture_stop(ref fp_dev dev, fp_capture_stop_cb callback, System.IntPtr user_data);

    }
}
