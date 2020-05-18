using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

namespace TestMyCode.CSharp.Core.Extensions
{
    internal static class BuildErrorEventArgsContext
    {
        internal static string GetNoobFriendlyTip(this BuildErrorEventArgs @this)
        {
            return @this.Code switch
            {
                "CS0103" => "Did you name your classes incorrectly or forgot to add correct imports?",
                _ => string.Empty
            };
        }
    }
}
