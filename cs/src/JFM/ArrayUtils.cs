using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM {
	public static class ArrayUtils {
		public static T[] ArrayCast<T>(this object[] array) {
			var dest = new T[array.Length];
			for(var i = 0; i < array.Length; i++)
				dest[i] = (T)array[i];
			return dest;
		}

		public static T[] Combine<T>(params T[][] sources) {
			int sourceCount = sources.Length;

			var destLength = 0;
			for(var i = 0; i < sourceCount; i++)
				destLength += sources[i].Length;

			var copied = 0;
			var dest = new T[destLength];
			for(var i = 0; i < sourceCount; i++) {
				var source = sources[i];
				Array.Copy(source, 0, dest, copied, source.Length);
				copied += source.Length;
			}

			return dest;
		}
	}
}
