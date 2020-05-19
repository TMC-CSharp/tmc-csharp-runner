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
                    return "Did you name your classes incorrectly or forget to add correct imports?";
                case "CS1026":
                case "CS1513":
                    return "Make sure all opening brackets have a corresponding closing bracket.";
                case "CS1003":
                    return @this.Message.Contains("']'")
                        ? "Make sure all opening brackets have a corresponding closing bracket."
                        : string.Empty;
                case "CS0116":
                    return "Make sure your variables and methods are inside a class or struct.";
                case "CS0165":
                    return "Make sure the variable is assigned a value.";
                case "CS0269":
                    return "Make sure the parameter is assigned a value.";
                case "CS1001":
                    return "Make sure your classes and variables have names.";
                case "CS1009":
                    return "If you want to use the backslash (\\) character in a string, type '\\\\' instead or precede the string with '@'.";
                case "CS1061":
                    return "Make sure the method or class member exists.";
                case "CS1501":
                case "CS1502":
                case "CS1503":
                    return "Make sure your arguments match the method definition.";
                case "CS1729":
                    return "Make sure your parameters match the constructor definition.";
                default:
                    return string.Empty;
            };
        }
    }
}
