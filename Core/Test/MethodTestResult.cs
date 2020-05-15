using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Runners;

namespace TestMyCode.Csharp.Core.Test
{
    public class MethodTestResult
    {
        private static readonly HashSet<string> EMPTY_HASH_SET = new HashSet<string>(0);

        public bool Passed { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public HashSet<string> Points { get; set; } = MethodTestResult.EMPTY_HASH_SET;

        public string ErrorStackTrace { get; set; } = string.Empty;

        internal MethodTestResult()
        {

        }

        internal static MethodTestResult FromSuccess(TestPassedInfo info, HashSet<string>? points = default)
        {
            return new MethodTestResult()
            {
                Passed = true,

                Name = info.TestDisplayName,

                Points = points ?? MethodTestResult.EMPTY_HASH_SET
            };
        }

        internal static MethodTestResult FromFail(TestFailedInfo info)
        {
            return new MethodTestResult()
            {
                Passed = false,

                Name = info.TestDisplayName,
                Message = info.ExceptionMessage,

                ErrorStackTrace = info.ExceptionStackTrace
            };
        }
    }
}
