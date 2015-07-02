using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
    public class EnrollStageResult
    {
        public EnrollStageResult(int enrollCode, byte[] fingerprintData)
        {
            this.ResultCode = enrollCode;
            
            if (this.ResultCode < 0)
                this.Result = EnrollResult.Unspecified;
            else
                this.Result = (EnrollResult)enrollCode;

            this.FingerprintData = fingerprintData;
        }

        public int ResultCode { get; protected set; }
        public EnrollResult Result { get; protected set; }
        public byte[] FingerprintData { get; protected set; }

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
        public bool IsFatalError { get { return ResultCode < 0; } }
        public bool IsSuccess { get { return IsFatalError == false && (Result == EnrollResult.EnrollComplete || Result == EnrollResult.EnrollStagePassed); } }
        public bool IsEnrollFailure { get { return IsFatalError == false && Result == EnrollResult.EnrollFailed; } }
        public bool IsEnrollComplete { get { return IsFatalError == false && Result == EnrollResult.EnrollComplete; } }
        public bool RequiresNewCall
        {
            get { return RequiresRetry || Result == EnrollResult.EnrollStagePassed; }
        }

    }
}
