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
            switch (@this.Code)
            {
                case "CS0103":
                    return "Did you name your classes incorrectly or forgot to add correct imports?";
                case "CS1026":
                case "CS1513":
                    return "Make sure you have the same number of opening and closing brackets.";
                case "CS1003":
                    return @this.Message.Contains("']'")
                        ? "Make sure you have the same number of opening and closing brackets." 
                        : string.Empty;
                default:
                    return string.Empty;
            };
        }
    }
}
