using System;

namespace Unosquare.Labs.LibFprint
{
    partial class Interop
    {
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct GSList
        {

            /// gpointer->void*
            public IntPtr data;

            /// GSList*
            public IntPtr next;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct libusb_device_handle
        {
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct libusb_device_descriptor
        {

            /// uint8_t->unsigned char
            public byte bLength;

            /// uint8_t->unsigned char
            public byte bDescriptorType;

            /// uint16_t->unsigned short
            public ushort bcdUSB;

            /// uint8_t->unsigned char
            public byte bDeviceClass;

            /// uint8_t->unsigned char
            public byte bDeviceSubClass;

            /// uint8_t->unsigned char
            public byte bDeviceProtocol;

            /// uint8_t->unsigned char
            public byte bMaxPacketSize0;

            /// uint16_t->unsigned short
            public ushort idVendor;

            /// uint16_t->unsigned short
            public ushort idProduct;

            /// uint16_t->unsigned short
            public ushort bcdDevice;

            /// uint8_t->unsigned char
            public byte iManufacturer;

            /// uint8_t->unsigned char
            public byte iProduct;

            /// uint8_t->unsigned char
            public byte iSerialNumber;

            /// uint8_t->unsigned char
            public byte bNumConfigurations;
        }

        public enum fp_dev_state
        {

            /// DEV_STATE_INITIAL -> 0
            DEV_STATE_INITIAL = 0,

            DEV_STATE_ERROR,

            DEV_STATE_INITIALIZING,

            DEV_STATE_INITIALIZED,

            DEV_STATE_DEINITIALIZING,

            DEV_STATE_DEINITIALIZED,

            DEV_STATE_ENROLL_STARTING,

            DEV_STATE_ENROLLING,

            DEV_STATE_ENROLL_STOPPING,

            DEV_STATE_VERIFY_STARTING,

            DEV_STATE_VERIFYING,

            DEV_STATE_VERIFY_DONE,

            DEV_STATE_VERIFY_STOPPING,

            DEV_STATE_IDENTIFY_STARTING,

            DEV_STATE_IDENTIFYING,

            DEV_STATE_IDENTIFY_DONE,

            DEV_STATE_IDENTIFY_STOPPING,

            DEV_STATE_CAPTURE_STARTING,

            DEV_STATE_CAPTURING,

            DEV_STATE_CAPTURE_DONE,

            DEV_STATE_CAPTURE_STOPPING,
        }

        public enum fp_driver_type
        {

            /// DRIVER_PRIMITIVE -> 0
            DRIVER_PRIMITIVE = 0,

            /// DRIVER_IMAGING -> 1
            DRIVER_IMAGING = 1,
        }

        public enum fp_print_data_type
        {

            /// PRINT_DATA_RAW -> 0
            PRINT_DATA_RAW = 0,

            PRINT_DATA_NBIS_MINUTIAE,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_dscv_dev
        {

            /// libusb_device*
            public IntPtr udev;

            /// fp_driver*
            public IntPtr drv;

            /// unsigned int
            public uint driver_data;

            /// uint32_t->unsigned int
            public uint devtype;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_dscv_print
        {

            /// uint16_t->unsigned short
            public ushort driver_id;

            /// uint32_t->unsigned int
            public uint devtype;

            /// fp_finger
            public fp_finger finger;

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string path;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_dev
        {

            /// fp_driver*
            public IntPtr drv;

            /// libusb_device_handle*
            public IntPtr udev;

            /// uint32_t->unsigned int
            public uint devtype;

            /// void*
            public IntPtr priv;

            /// int
            public int nr_enroll_stages;

            /// fp_print_data*
            public IntPtr verify_data;

            /// fp_dev_state
            public fp_dev_state state;

            /// int
            public int @__enroll_stage;

            /// int
            public int unconditional_capture;

            /// fp_dev_open_cb
            public fp_dev_open_cb open_cb;

            /// void*
            public IntPtr open_cb_data;

            /// fp_dev_close_cb
            public fp_dev_close_cb close_cb;

            /// void*
            public IntPtr close_cb_data;

            /// fp_enroll_stage_cb
            public fp_enroll_stage_cb enroll_stage_cb;

            /// void*
            public IntPtr enroll_stage_cb_data;

            /// fp_enroll_stop_cb
            public fp_enroll_stop_cb enroll_stop_cb;

            /// void*
            public IntPtr enroll_stop_cb_data;

            /// fp_verify_cb
            public fp_verify_cb verify_cb;

            /// void*
            public IntPtr verify_cb_data;

            /// fp_verify_stop_cb
            public fp_verify_stop_cb verify_stop_cb;

            /// void*
            public IntPtr verify_stop_cb_data;

            /// fp_identify_cb
            public fp_identify_cb identify_cb;

            /// void*
            public IntPtr identify_cb_data;

            /// fp_identify_stop_cb
            public fp_identify_stop_cb identify_stop_cb;

            /// void*
            public IntPtr identify_stop_cb_data;

            /// fp_capture_cb
            public fp_capture_cb capture_cb;

            /// void*
            public IntPtr capture_cb_data;

            /// fp_capture_stop_cb
            public fp_capture_stop_cb capture_stop_cb;

            /// void*
            public IntPtr capture_stop_cb_data;

            /// fp_print_data**
            public IntPtr identify_gallery;
        }

        /// Return Type: int
        ///dsc: libusb_device_descriptor*
        ///devtype: uint32_t*
        public delegate int fp_driver_discover(ref libusb_device_descriptor dsc, ref uint devtype);

        /// Return Type: int
        ///dev: fp_dev*
        ///driver_data: unsigned int
        public delegate int fp_driver_open(ref fp_dev dev, uint driver_data);

        /// Return Type: void
        ///dev: fp_dev*
        public delegate void fp_driver_close(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_enroll_start(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_enroll_stop(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_verify_start(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        ///iterating: gboolean->gint->int
        public delegate int fp_driver_verify_stop(ref fp_dev dev, int iterating);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_identify_start(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        ///iterating: gboolean->gint->int
        public delegate int fp_driver_identify_stop(ref fp_dev dev, int iterating);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_capture_start(ref fp_dev dev);

        /// Return Type: int
        ///dev: fp_dev*
        public delegate int fp_driver_capture_stop(ref fp_dev dev);

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_driver
        {

            /// uint16_t->unsigned short
            public ushort id;

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string name;

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string full_name;

            /// usb_id*
            public IntPtr id_table;

            /// fp_driver_type
            public fp_driver_type type;

            /// fp_scan_type
            public fp_scan_type scan_type;

            /// void*
            public IntPtr priv;

            /// fp_driver_discover
            public fp_driver_discover AnonymousMember1;

            /// fp_driver_open
            public fp_driver_open AnonymousMember2;

            /// fp_driver_close
            public fp_driver_close AnonymousMember3;

            /// fp_driver_enroll_start
            public fp_driver_enroll_start AnonymousMember4;

            /// fp_driver_enroll_stop
            public fp_driver_enroll_stop AnonymousMember5;

            /// fp_driver_verify_start
            public fp_driver_verify_start AnonymousMember6;

            /// fp_driver_verify_stop
            public fp_driver_verify_stop AnonymousMember7;

            /// fp_driver_identify_start
            public fp_driver_identify_start AnonymousMember8;

            /// fp_driver_identify_stop
            public fp_driver_identify_stop AnonymousMember9;

            /// fp_driver_capture_start
            public fp_driver_capture_start AnonymousMember10;

            /// fp_driver_capture_stop
            public fp_driver_capture_stop AnonymousMember11;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_print_data
        {

            /// uint16_t->unsigned short
            public ushort driver_id;

            /// uint32_t->unsigned int
            public uint devtype;

            /// fp_print_data_type
            public fp_print_data_type type;

            /// GSList*
            public IntPtr prints;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct fp_img
        {

            /// int
            public int width;

            /// int
            public int height;

            /// size_t->unsigned int
            public uint length;

            /// uint16_t->unsigned short
            public ushort flags;

            /// fp_minutiae*
            public IntPtr minutiae;

            /// unsigned char*
            public IntPtr binarized;

            /// unsigned char[0]
            public IntPtr data;
        }

        public enum fp_finger
        {

            /// LEFT_THUMB -> 1
            LEFT_THUMB = 1,

            LEFT_INDEX,

            LEFT_MIDDLE,

            LEFT_RING,

            LEFT_LITTLE,

            RIGHT_THUMB,

            RIGHT_INDEX,

            RIGHT_MIDDLE,

            RIGHT_RING,

            RIGHT_LITTLE,
        }

        public enum fp_scan_type
        {

            /// FP_SCAN_TYPE_PRESS -> 0
            FP_SCAN_TYPE_PRESS = 0,

            FP_SCAN_TYPE_SWIPE,
        }

        public enum fp_capture_result
        {

            /// FP_CAPTURE_COMPLETE -> 0
            FP_CAPTURE_COMPLETE = 0,

            FP_CAPTURE_FAIL,
        }

        public enum fp_enroll_result
        {

            /// FP_ENROLL_COMPLETE -> 1
            FP_ENROLL_COMPLETE = 1,

            FP_ENROLL_FAIL,

            FP_ENROLL_PASS,

            /// FP_ENROLL_RETRY -> 100
            FP_ENROLL_RETRY = 100,

            FP_ENROLL_RETRY_TOO_SHORT,

            FP_ENROLL_RETRY_CENTER_FINGER,

            FP_ENROLL_RETRY_REMOVE_FINGER,
        }

        public enum fp_verify_result
        {

            /// FP_VERIFY_NO_MATCH -> 0
            FP_VERIFY_NO_MATCH = 0,

            /// FP_VERIFY_MATCH -> 1
            FP_VERIFY_MATCH = 1,

            /// FP_VERIFY_RETRY -> FP_ENROLL_RETRY
            FP_VERIFY_RETRY = fp_enroll_result.FP_ENROLL_RETRY,

            /// FP_VERIFY_RETRY_TOO_SHORT -> FP_ENROLL_RETRY_TOO_SHORT
            FP_VERIFY_RETRY_TOO_SHORT = fp_enroll_result.FP_ENROLL_RETRY_TOO_SHORT,

            /// FP_VERIFY_RETRY_CENTER_FINGER -> FP_ENROLL_RETRY_CENTER_FINGER
            FP_VERIFY_RETRY_CENTER_FINGER = fp_enroll_result.FP_ENROLL_RETRY_CENTER_FINGER,

            /// FP_VERIFY_RETRY_REMOVE_FINGER -> FP_ENROLL_RETRY_REMOVE_FINGER
            FP_VERIFY_RETRY_REMOVE_FINGER = fp_enroll_result.FP_ENROLL_RETRY_REMOVE_FINGER,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_minutia
        {

            /// int
            public int x;

            /// int
            public int y;

            /// int
            public int ex;

            /// int
            public int ey;

            /// int
            public int direction;

            /// double
            public double reliability;

            /// int
            public int type;

            /// int
            public int appearing;

            /// int
            public int feature_id;

            /// int*
            public IntPtr nbrs;

            /// int*
            public IntPtr ridge_counts;

            /// int
            public int num_nbrs;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct fp_pollfd
        {

            /// int
            public int fd;

            /// short
            public short events;
        }

        /// Return Type: void
        ///fd: int
        ///events: short
        public delegate void fp_pollfd_added_cb(int fd, short events);

        /// Return Type: void
        ///fd: int
        public delegate void fp_pollfd_removed_cb(int fd);

        /// Return Type: void
        ///dev: fp_dev*
        ///status: int
        ///user_data: void*
        public delegate void fp_dev_open_cb(ref fp_dev dev, int status, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///user_data: void*
        public delegate void fp_dev_close_cb(ref fp_dev dev, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///result: int
        ///print: fp_print_data*
        ///img: fp_img*
        ///user_data: void*
        public delegate void fp_enroll_stage_cb(ref fp_dev dev, int result, ref fp_print_data print, ref fp_img img, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///user_data: void*
        public delegate void fp_enroll_stop_cb(ref fp_dev dev, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///result: int
        ///img: fp_img*
        ///user_data: void*
        public delegate void fp_verify_cb(ref fp_dev dev, int result, ref fp_img img, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///user_data: void*
        public delegate void fp_verify_stop_cb(ref fp_dev dev, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///result: int
        ///match_offset: size_t->unsigned int
        ///img: fp_img*
        ///user_data: void*
        public delegate void fp_identify_cb(ref fp_dev dev, int result, IntPtr match_offset, ref fp_img img, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///user_data: void*
        public delegate void fp_identify_stop_cb(ref fp_dev dev, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///result: int
        ///img: fp_img*
        ///user_data: void*
        public delegate void fp_capture_cb(ref fp_dev dev, int result, ref fp_img img, IntPtr user_data);

        /// Return Type: void
        ///dev: fp_dev*
        ///user_data: void*
        public delegate void fp_capture_stop_cb(ref fp_dev dev, IntPtr user_data);

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct timeval
        {

            /// int
            public int tv_sec;

            /// int
            public int tv_usec;
        }
    }
}
