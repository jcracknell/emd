using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
  public static class StringUtils {
    private static readonly char[] LITERALENCODE_ESCAPE_CHARS;

    static StringUtils() {
      // Initialize array of characters to be escaped during literal encoding
      // Per http://msdn.microsoft.com/en-us/library/h21280bw.aspx
      LITERALENCODE_ESCAPE_CHARS = new char[93];
      LITERALENCODE_ESCAPE_CHARS['\a'] = 'a';
      LITERALENCODE_ESCAPE_CHARS['\b'] = 'b';
      LITERALENCODE_ESCAPE_CHARS['\f'] = 'f';
      LITERALENCODE_ESCAPE_CHARS['\n'] = 'n';
      LITERALENCODE_ESCAPE_CHARS['\r'] = 'r';
      LITERALENCODE_ESCAPE_CHARS['\t'] = 't';
      LITERALENCODE_ESCAPE_CHARS['"'] = '"';
      LITERALENCODE_ESCAPE_CHARS['\\'] = '\\';
      LITERALENCODE_ESCAPE_CHARS['?'] = '?';
    }

    /// <summary>
    /// Convert the string to the equivalent C# string literal, enclosing the string in double quotes and inserting
    /// escape sequences as necessary.
    /// </summary>
    /// <param name="s">The string to be converted to a C# string literal.</param>
    /// <returns><paramref name="s"/> represented as a C# string literal.</returns>
    public static string LiteralEncode(string s) {
      if(null == s) return "null";

      var len = s.Length;
      var sb = new StringBuilder("\"", len * 2 + 2);
      for(var rp = 0; rp < len; rp++) {
        var c = s[rp];
        if(c < LITERALENCODE_ESCAPE_CHARS.Length && 0 != LITERALENCODE_ESCAPE_CHARS[c]) {
          sb.Append('\\').Append(LITERALENCODE_ESCAPE_CHARS[c]);
        } else if(' ' <= c && c <= '~' || char.IsLetter(c)) {
          sb.Append(c);
        } else {
          sb.Append(@"\x").Append(CharUtils.AsHex(c));
        }
      }
      
      return sb.Append("\"").ToString();
    }
  }
}
