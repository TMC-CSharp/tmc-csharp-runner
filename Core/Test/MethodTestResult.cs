using System;
using System.Collections.Generic;
using System.Text;

namespace TestMyCode.Csharp.Core.Test
{
    public class MethodTestResult
    {
        public bool Passed { get; set; }

        public string Name { get; set; }

        public string[] Points { get; set; }

        public string Message { get; set; }

        public string ErrorStackTrace { get; set; }
    }
}
