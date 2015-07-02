namespace Unosquare.Labs.LibFprint
{
    /// <summary>
    /// An enumeration that represents enrollment results.
    /// </summary>
    public enum EnrollResult
    {
        Unspecified = 0,
        EnrollComplete = 1,
        EnrollFailed = 2,
        EnrollStagePassed = 3,
        Retry = 100,
        RetryScanTooShort = 101,
        RetryFingerNotCentered = 102,
        RetryRemoveFinger = 103,
    }
}
