// We are keeping this file for reference only because we generated a bunch of code from this "slightly" modified version :)

typedef unsigned short uint16_t;
typedef unsigned long uint32_t;
typedef unsigned char uint8_t;
typedef int    gint;
typedef gint   gboolean;

typedef void* gpointer;

struct GSList {
  gpointer data;
  GSList *next;
};

typedef struct libusb_device_handle { } libusb_device_handle;

struct libusb_device_descriptor {
	/** Size of this descriptor (in bytes) */
	uint8_t  bLength;

	/** Descriptor type. Will have value
	 * \ref libusb_descriptor_type::LIBUSB_DT_DEVICE LIBUSB_DT_DEVICE in this
	 * context. */
	uint8_t  bDescriptorType;

	/** USB specification release number in binary-coded decimal. A value of
	 * 0x0200 indicates USB 2.0, 0x0110 indicates USB 1.1, etc. */
	uint16_t bcdUSB;

	/** USB-IF class code for the device. See \ref libusb_class_code. */
	uint8_t  bDeviceClass;

	/** USB-IF subclass code for the device, qualified by the bDeviceClass
	 * value */
	uint8_t  bDeviceSubClass;

	/** USB-IF protocol code for the device, qualified by the bDeviceClass and
	 * bDeviceSubClass values */
	uint8_t  bDeviceProtocol;

	/** Maximum packet size for endpoint 0 */
	uint8_t  bMaxPacketSize0;

	/** USB-IF vendor ID */
	uint16_t idVendor;

	/** USB-IF product ID */
	uint16_t idProduct;

	/** Device release number in binary-coded decimal */
	uint16_t bcdDevice;

	/** Index of string descriptor describing manufacturer */
	uint8_t  iManufacturer;

	/** Index of string descriptor describing product */
	uint8_t  iProduct;

	/** Index of string descriptor containing device serial number */
	uint8_t  iSerialNumber;

	/** Number of possible configurations */
	uint8_t  bNumConfigurations;
};

/* structs that applications are not allowed to peek into */
enum fp_dev_state {
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
};

enum fp_driver_type {
	DRIVER_PRIMITIVE = 0,
	DRIVER_IMAGING = 1,
};

enum fp_print_data_type {
	PRINT_DATA_RAW = 0, /* memset-imposed default */
	PRINT_DATA_NBIS_MINUTIAE,
};


struct fp_dscv_dev {
	struct libusb_device *udev;
	struct fp_driver *drv;
	unsigned long driver_data;
	uint32_t devtype;
};
struct fp_dscv_print {
	uint16_t driver_id;
	uint32_t devtype;
	enum fp_finger finger;
	char *path;
};
struct fp_dev {
	struct fp_driver *drv;
	libusb_device_handle *udev;
	uint32_t devtype;
	void *priv;

	int nr_enroll_stages;

	/* read-only to drivers */
	struct fp_print_data *verify_data;

	/* drivers should not mess with any of the below */
	enum fp_dev_state state;
	int __enroll_stage;
	int unconditional_capture;

	/* async I/O callbacks and data */
	/* FIXME: convert this to generic state operational data mechanism? */
	fp_dev_open_cb open_cb;
	void *open_cb_data;
	fp_dev_close_cb close_cb;
	void *close_cb_data;
	fp_enroll_stage_cb enroll_stage_cb;
	void *enroll_stage_cb_data;
	fp_enroll_stop_cb enroll_stop_cb;
	void *enroll_stop_cb_data;
	fp_verify_cb verify_cb;
	void *verify_cb_data;
	fp_verify_stop_cb verify_stop_cb;
	void *verify_stop_cb_data;
	fp_identify_cb identify_cb;
	void *identify_cb_data;
	fp_identify_stop_cb identify_stop_cb;
	void *identify_stop_cb_data;
	fp_capture_cb capture_cb;
	void *capture_cb_data;
	fp_capture_stop_cb capture_stop_cb;
	void *capture_stop_cb_data;

	/* FIXME: better place to put this? */
	struct fp_print_data **identify_gallery;
};
struct fp_driver {
	const uint16_t id;
	const char *name;
	const char *full_name;
	const struct usb_id * const id_table;
	enum fp_driver_type type;
	enum fp_scan_type scan_type;

	void *priv;

	/* Device operations */
	int (*discover)(struct libusb_device_descriptor *dsc, uint32_t *devtype);
	int (*open)(struct fp_dev *dev, unsigned long driver_data);
	void (*close)(struct fp_dev *dev);
	int (*enroll_start)(struct fp_dev *dev);
	int (*enroll_stop)(struct fp_dev *dev);
	int (*verify_start)(struct fp_dev *dev);
	int (*verify_stop)(struct fp_dev *dev, gboolean iterating);
	int (*identify_start)(struct fp_dev *dev);
	int (*identify_stop)(struct fp_dev *dev, gboolean iterating);
	int (*capture_start)(struct fp_dev *dev);
	int (*capture_stop)(struct fp_dev *dev);
};
struct fp_print_data {
	uint16_t driver_id;
	uint32_t devtype;
	enum fp_print_data_type type;
	GSList *prints;
};
struct fp_img {
	int width;
	int height;
	size_t length;
	uint16_t flags;
	struct fp_minutiae *minutiae;
	unsigned char *binarized;
	unsigned char data[0];
};

/* misc/general stuff */

/** \ingroup print_data
 * Numeric codes used to refer to fingers (and thumbs) of a human. These are
 * purposely not available as strings, to avoid getting the library tangled up
 * in localization efforts.
 */
enum fp_finger {
	LEFT_THUMB = 1, /** thumb (left hand) */
	LEFT_INDEX, /** index finger (left hand) */
	LEFT_MIDDLE, /** middle finger (left hand) */
	LEFT_RING, /** ring finger (left hand) */
	LEFT_LITTLE, /** little finger (left hand) */
	RIGHT_THUMB, /** thumb (right hand) */
	RIGHT_INDEX, /** index finger (right hand) */
	RIGHT_MIDDLE, /** middle finger (right hand) */
	RIGHT_RING, /** ring finger (right hand) */
	RIGHT_LITTLE, /** little finger (right hand) */
};

/** \ingroup dev
 * Numeric codes used to refer to the scan type of the device. Devices require
 * either swiping or pressing the finger on the device. This is useful for
 * front-ends.
 */
enum fp_scan_type {
	FP_SCAN_TYPE_PRESS = 0, /** press */
	FP_SCAN_TYPE_SWIPE, /** swipe */
};

/* Drivers */
const char *fp_driver_get_name(struct fp_driver *drv);
const char *fp_driver_get_full_name(struct fp_driver *drv);
uint16_t fp_driver_get_driver_id(struct fp_driver *drv);
enum fp_scan_type fp_driver_get_scan_type(struct fp_driver *drv);

/* Device discovery */
struct fp_dscv_dev **fp_discover_devs(void);
void fp_dscv_devs_free(struct fp_dscv_dev **devs);
struct fp_driver *fp_dscv_dev_get_driver(struct fp_dscv_dev *dev);
uint32_t fp_dscv_dev_get_devtype(struct fp_dscv_dev *dev);
int fp_dscv_dev_supports_print_data(struct fp_dscv_dev *dev,
	struct fp_print_data *print);
int fp_dscv_dev_supports_dscv_print(struct fp_dscv_dev *dev,
	struct fp_dscv_print *print);
struct fp_dscv_dev *fp_dscv_dev_for_print_data(struct fp_dscv_dev **devs,
	struct fp_print_data *print);
struct fp_dscv_dev *fp_dscv_dev_for_dscv_print(struct fp_dscv_dev **devs,
	struct fp_dscv_print *print);

static inline uint16_t fp_dscv_dev_get_driver_id(struct fp_dscv_dev *dev);

/* Print discovery */
struct fp_dscv_print **fp_discover_prints(void);
void fp_dscv_prints_free(struct fp_dscv_print **prints);
uint16_t fp_dscv_print_get_driver_id(struct fp_dscv_print *print);
uint32_t fp_dscv_print_get_devtype(struct fp_dscv_print *print);
enum fp_finger fp_dscv_print_get_finger(struct fp_dscv_print *print);
int fp_dscv_print_delete(struct fp_dscv_print *print);

/* Device handling */
struct fp_dev *fp_dev_open(struct fp_dscv_dev *ddev);
void fp_dev_close(struct fp_dev *dev);
struct fp_driver *fp_dev_get_driver(struct fp_dev *dev);
int fp_dev_get_nr_enroll_stages(struct fp_dev *dev);
uint32_t fp_dev_get_devtype(struct fp_dev *dev);
int fp_dev_supports_print_data(struct fp_dev *dev, struct fp_print_data *data);
int fp_dev_supports_dscv_print(struct fp_dev *dev, struct fp_dscv_print *print);

/** \ingroup dev
 * Image capture result codes returned from fp_dev_img_capture().
 */
enum fp_capture_result {
	/** Capture completed successfully, the capture data has been
	 * returned to the caller. */
	FP_CAPTURE_COMPLETE = 0,
	/** Capture failed for some reason */
	FP_CAPTURE_FAIL,
};

int fp_dev_supports_imaging(struct fp_dev *dev);
int fp_dev_img_capture(struct fp_dev *dev, int unconditional,
	struct fp_img **image);
int fp_dev_get_img_width(struct fp_dev *dev);
int fp_dev_get_img_height(struct fp_dev *dev);

/** \ingroup dev
 * Enrollment result codes returned from fp_enroll_finger().
 * Result codes with RETRY in the name suggest that the scan failed due to
 * user error. Applications will generally want to inform the user of the
 * problem and then retry the enrollment stage. For more info on the semantics
 * of interpreting these result codes and tracking enrollment process, see
 * \ref enrolling.
 */
enum fp_enroll_result {
	/** Enrollment completed successfully, the enrollment data has been
	 * returned to the caller. */
	FP_ENROLL_COMPLETE = 1,
	/** Enrollment failed due to incomprehensible data; this may occur when
	 * the user scans a different finger on each enroll stage. */
	FP_ENROLL_FAIL,
	/** Enroll stage passed; more stages are need to complete the process. */
	FP_ENROLL_PASS,
	/** The enrollment scan did not succeed due to poor scan quality or
	 * other general user scanning problem. */
	FP_ENROLL_RETRY = 100,
	/** The enrollment scan did not succeed because the finger swipe was
	 * too short. */
	FP_ENROLL_RETRY_TOO_SHORT,
	/** The enrollment scan did not succeed because the finger was not
	 * centered on the scanner. */
	FP_ENROLL_RETRY_CENTER_FINGER,
	/** The verification scan did not succeed due to quality or pressure
	 * problems; the user should remove their finger from the scanner before
	 * retrying. */
	FP_ENROLL_RETRY_REMOVE_FINGER,
};

int fp_enroll_finger_img(struct fp_dev *dev, struct fp_print_data **print_data,
	struct fp_img **img);

/** \ingroup dev
 * Performs an enroll stage. See \ref enrolling for an explanation of enroll
 * stages. This function is just a shortcut to calling fp_enroll_finger_img()
 * with a NULL image parameter. Be sure to read the description of
 * fp_enroll_finger_img() in order to understand its behaviour.
 *
 * \param dev the device
 * \param print_data a location to return the resultant enrollment data from
 * the final stage. Must be freed with fp_print_data_free() after use.
 * \return negative code on error, otherwise a code from #fp_enroll_result
 */
static inline int fp_enroll_finger(struct fp_dev *dev,
	struct fp_print_data **print_data);

/** \ingroup dev
 * Verification result codes returned from fp_verify_finger(). Return codes
 * are also shared with fp_identify_finger().
 * Result codes with RETRY in the name suggest that the scan failed due to
 * user error. Applications will generally want to inform the user of the
 * problem and then retry the verify operation.
 */
enum fp_verify_result {
	/** The scan completed successfully, but the newly scanned fingerprint
	 * does not match the fingerprint being verified against.
	 * In the case of identification, this return code indicates that the
	 * scanned finger could not be found in the print gallery. */
	FP_VERIFY_NO_MATCH = 0,
	/** The scan completed successfully and the newly scanned fingerprint does
	 * match the fingerprint being verified, or in the case of identification,
	 * the scanned fingerprint was found in the print gallery. */
	FP_VERIFY_MATCH = 1,
	/** The scan did not succeed due to poor scan quality or other general
	 * user scanning problem. */
	FP_VERIFY_RETRY = FP_ENROLL_RETRY,
	/** The scan did not succeed because the finger swipe was too short. */
	FP_VERIFY_RETRY_TOO_SHORT = FP_ENROLL_RETRY_TOO_SHORT,
	/** The scan did not succeed because the finger was not centered on the
	 * scanner. */
	FP_VERIFY_RETRY_CENTER_FINGER = FP_ENROLL_RETRY_CENTER_FINGER,
	/** The scan did not succeed due to quality or pressure problems; the user
	 * should remove their finger from the scanner before retrying. */
	FP_VERIFY_RETRY_REMOVE_FINGER = FP_ENROLL_RETRY_REMOVE_FINGER,
};

int fp_verify_finger_img(struct fp_dev *dev,
	struct fp_print_data *enrolled_print, struct fp_img **img);

/** \ingroup dev
 * Performs a new scan and verify it against a previously enrolled print. This
 * function is just a shortcut to calling fp_verify_finger_img() with a NULL
 * image output parameter.
 * \param dev the device to perform the scan.
 * \param enrolled_print the print to verify against. Must have been previously
 * enrolled with a device compatible to the device selected to perform the scan.
 * \return negative code on error, otherwise a code from #fp_verify_result
 * \sa fp_verify_finger_img()
 */
static inline int fp_verify_finger(struct fp_dev *dev,
	struct fp_print_data *enrolled_print);

int fp_dev_supports_identification(struct fp_dev *dev);
int fp_identify_finger_img(struct fp_dev *dev,
	struct fp_print_data **print_gallery, size_t *match_offset,
	struct fp_img **img);

/** \ingroup dev
 * Performs a new scan and attempts to identify the scanned finger against a
 * collection of previously enrolled fingerprints. This function is just a
 * shortcut to calling fp_identify_finger_img() with a NULL image output
 * parameter.
 * \param dev the device to perform the scan.
 * \param print_gallery NULL-terminated array of pointers to the prints to
 * identify against. Each one must have been previously enrolled with a device
 * compatible to the device selected to perform the scan.
 * \param match_offset output location to store the array index of the matched
 * gallery print (if any was found). Only valid if FP_VERIFY_MATCH was
 * returned.
 * \return negative code on error, otherwise a code from #fp_verify_result
 * \sa fp_identify_finger_img()
 */
static inline int fp_identify_finger(struct fp_dev *dev,
	struct fp_print_data **print_gallery, size_t *match_offset);

/* Data handling */
int fp_print_data_load(struct fp_dev *dev, enum fp_finger finger,
	struct fp_print_data **data);
int fp_print_data_from_dscv_print(struct fp_dscv_print *print,
	struct fp_print_data **data);
int fp_print_data_save(struct fp_print_data *data, enum fp_finger finger);
int fp_print_data_delete(struct fp_dev *dev, enum fp_finger finger);
void fp_print_data_free(struct fp_print_data *data);
size_t fp_print_data_get_data(struct fp_print_data *data, unsigned char **ret);
struct fp_print_data *fp_print_data_from_data(unsigned char *buf,
	size_t buflen);
uint16_t fp_print_data_get_driver_id(struct fp_print_data *data);
uint32_t fp_print_data_get_devtype(struct fp_print_data *data);

/* Image handling */

/** \ingroup img */
struct fp_minutia {
	int x;
	int y;
	int ex;
	int ey;
	int direction;
	double reliability;
	int type;
	int appearing;
	int feature_id;
	int *nbrs;
	int *ridge_counts;
	int num_nbrs;
};

int fp_img_get_height(struct fp_img *img);
int fp_img_get_width(struct fp_img *img);
unsigned char *fp_img_get_data(struct fp_img *img);
int fp_img_save_to_file(struct fp_img *img, char *path);
void fp_img_standardize(struct fp_img *img);
struct fp_img *fp_img_binarize(struct fp_img *img);
struct fp_minutia **fp_img_get_minutiae(struct fp_img *img, int *nr_minutiae);
void fp_img_free(struct fp_img *img);

/* Polling and timing */

struct fp_pollfd {
	int fd;
	short events;
};

int fp_handle_events_timeout(struct timeval *timeout);
int fp_handle_events(void);
size_t fp_get_pollfds(struct fp_pollfd **pollfds);
int fp_get_next_timeout(struct timeval *tv);

typedef void (*fp_pollfd_added_cb)(int fd, short events);
typedef void (*fp_pollfd_removed_cb)(int fd);
void fp_set_pollfd_notifiers(fp_pollfd_added_cb added_cb,
	fp_pollfd_removed_cb removed_cb);

/* Library */
int fp_init(void);
void fp_exit(void);
void fp_set_debug(int level);

/* Asynchronous I/O */

typedef void (*fp_dev_open_cb)(struct fp_dev *dev, int status, void *user_data);
int fp_async_dev_open(struct fp_dscv_dev *ddev, fp_dev_open_cb callback,
	void *user_data);

typedef void (*fp_dev_close_cb)(struct fp_dev *dev, void *user_data);
void fp_async_dev_close(struct fp_dev *dev, fp_dev_close_cb callback,
	void *user_data);

typedef void (*fp_enroll_stage_cb)(struct fp_dev *dev, int result,
	struct fp_print_data *print, struct fp_img *img, void *user_data);
int fp_async_enroll_start(struct fp_dev *dev, fp_enroll_stage_cb callback,
	void *user_data);

typedef void (*fp_enroll_stop_cb)(struct fp_dev *dev, void *user_data);
int fp_async_enroll_stop(struct fp_dev *dev, fp_enroll_stop_cb callback,
	void *user_data);

typedef void (*fp_verify_cb)(struct fp_dev *dev, int result,
	struct fp_img *img, void *user_data);
int fp_async_verify_start(struct fp_dev *dev, struct fp_print_data *data,
	fp_verify_cb callback, void *user_data);

typedef void (*fp_verify_stop_cb)(struct fp_dev *dev, void *user_data);
int fp_async_verify_stop(struct fp_dev *dev, fp_verify_stop_cb callback,
	void *user_data);

typedef void (*fp_identify_cb)(struct fp_dev *dev, int result,
	size_t match_offset, struct fp_img *img, void *user_data);
int fp_async_identify_start(struct fp_dev *dev, struct fp_print_data **gallery,
	fp_identify_cb callback, void *user_data);

typedef void (*fp_identify_stop_cb)(struct fp_dev *dev, void *user_data);
int fp_async_identify_stop(struct fp_dev *dev, fp_identify_stop_cb callback,
	void *user_data);

typedef void (*fp_capture_cb)(struct fp_dev *dev, int result,
	struct fp_img *img, void *user_data);
int fp_async_capture_start(struct fp_dev *dev, int unconditional, fp_capture_cb callback, void *user_data);

typedef void (*fp_capture_stop_cb)(struct fp_dev *dev, void *user_data);
int fp_async_capture_stop(struct fp_dev *dev, fp_capture_stop_cb callback, void *user_data);

#endif

