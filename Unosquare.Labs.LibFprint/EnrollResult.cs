using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unosquare.Labs.LibFprint
{
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
