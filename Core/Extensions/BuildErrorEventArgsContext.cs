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
				"CS0103" => "Did you name your classes incorrectly or forget to add correct imports?",
				"CS1026" or "CS1513" => "Make sure all opening brackets have a corresponding closing bracket.",
				"CS1003" => @this.Message.Contains("']'") ? "Make sure all opening brackets have a corresponding closing bracket." : string.Empty,
				"CS0116" => "Make sure your variables and methods are inside a class or struct.",
				"CS0165" => "Make sure the variable is assigned a value.",
				"CS0269" => "Make sure the parameter is assigned a value.",
				"CS1001" => "Make sure your classes and variables have names.",
				"CS1009" => "If you want to use the backslash (\\) character in a string, type '\\\\' instead or precede the string with '@'.",
				"CS1061" => "Make sure the method or class member exists.",
				"CS1501" or "CS1502" or "CS1503" => "Make sure your arguments match the method definition.",
				"CS1729" => "Make sure your parameters match the constructor definition.",
				_ => string.Empty,
			};
        }
    }
}
