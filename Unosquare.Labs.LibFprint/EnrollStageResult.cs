namespace Unosquare.Labs.LibFprint
{
    /// <summary>
    /// Represents the state of a Fingerprint Enrollment operation
    /// </summary>
    public class EnrollStageResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollStageResult"/> class.
        /// </summary>
        /// <param name="enrollCode">The enroll code.</param>
        /// <param name="fingerprintData">The fingerprint data.</param>
        public EnrollStageResult(int enrollCode, byte[] fingerprintData)
        {
            ResultCode = enrollCode;

            Result = ResultCode < 0 ? EnrollResult.Unspecified : (EnrollResult) enrollCode;

            FingerprintData = fingerprintData;
        }

        /// <summary>
        /// Gets or sets the raw result code.
        /// </summary>
        /// <value>
        /// The result code.
        /// </value>
        public int ResultCode { get; protected set; }
        
        /// <summary>
        /// Gets or sets the friendly result code.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public EnrollResult Result { get; protected set; }
        
        /// <summary>
        /// Gets the buffer data holding the fingerprint scan.
        /// This is what needs to be saved in a database that will later be loased on to a
        /// FingerprintGallery object.
        /// </summary>
        /// <value>
        /// The fingerprint data.
        /// </value>
        public byte[] FingerprintData { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the enrollment requires retry.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires retry]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresRetry
        {
            get
            {
                if (IsFatalError) return false;

                return
                    Result == EnrollResult.Retry ||
                    Result == EnrollResult.RetryFingerNotCentered ||
                    Result == EnrollResult.RetryRemoveFinger ||
                    Result == EnrollResult.RetryScanTooShort;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the result contains a fatal error.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fatal error; otherwise, <c>false</c>.
        /// </value>
        public bool IsFatalError => ResultCode < 0;

        /// <summary>
        /// Gets a value indicating whether the result was a successful scan.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess => IsFatalError == false && (Result == EnrollResult.EnrollComplete || Result == EnrollResult.EnrollStagePassed);

        /// <summary>
        /// Gets a value indicating whether this instance represents an enrollment failure.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enroll failure; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnrollFailure => IsFatalError == false && Result == EnrollResult.EnrollFailed;

        /// <summary>
        /// Gets a value indicating whether this instance represents an enrollment completion.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enroll complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnrollComplete => IsFatalError == false && Result == EnrollResult.EnrollComplete;

        /// <summary>
        /// Gets a value indicating whether a new call to the Enroll Fingerprint method is required to advance or to retry.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires new call]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresNewCall => RequiresRetry || Result == EnrollResult.EnrollStagePassed;
    }
}
