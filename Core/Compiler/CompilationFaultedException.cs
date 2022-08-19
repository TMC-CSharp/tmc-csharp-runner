using System;
using System.Collections.Generic;

namespace TestMyCode.CSharp.Core.Compiler
{
    public class CompilationFaultedException : Exception
    {
        internal CompilationFaultedException(IReadOnlyList<string> compilationErrors)
            : base(string.Join(Environment.NewLine, compilationErrors))
        {
        }
    }
}
