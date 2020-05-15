using System;
using System.Collections.Generic;
using System.Text;

namespace TestMyCode.Csharp.Core.Compiler
{
    public class CompilationFaultedException : Exception
    {
        internal CompilationFaultedException(IReadOnlyList<string> compilationErrors)
            : base(string.Join(Environment.NewLine, compilationErrors))
        {
        }
    }
}
