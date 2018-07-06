namespace Unosquare.Labs.LibFprint
{
    /// <summary>
    /// An enumeration that represents enrollment results.
    /// </summary>
    public enum EnrollResult
    {
        /// <summary>
        /// The unspecified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The enroll complete
        /// </summary>
        EnrollComplete = 1,

        /// <summary>
        /// The enroll failed
        /// </summary>
        EnrollFailed = 2,

        /// <summary>
        /// The enroll stage passed
        /// </summary>
        EnrollStagePassed = 3,

        /// <summary>
        /// The retry
        /// </summary>
        Retry = 100,

        /// <summary>
        /// The retry scan too short
        /// </summary>
        RetryScanTooShort = 101,

        /// <summary>
        /// The retry finger not centered
        /// </summary>
        RetryFingerNotCentered = 102,

        /// <summary>
        /// The retry remove finger
        /// </summary>
        RetryRemoveFinger = 103,
    }
}
