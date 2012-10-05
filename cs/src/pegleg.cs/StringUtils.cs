using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pegleg.cs {
	internal static class StringUtils {
		public static string LiteralEncode(string s) {
			using(var stringWriter = new StringWriter())
			using(var csharpCodeProvider = new CSharpCodeProvider()) {
				var primitiveExpression = new CodePrimitiveExpression(s);
				csharpCodeProvider.GenerateCodeFromExpression(primitiveExpression, stringWriter, null);
				return stringWriter.GetStringBuilder().ToString();
			}
		}
	}
}
