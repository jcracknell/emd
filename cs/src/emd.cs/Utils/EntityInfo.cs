using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Utils {
	public class EntityInfo {
		private static readonly IDictionary<string, int[]> _codepointsByEntityName;
		private static readonly IDictionary<int, string> _entityNameByCodepoint;

		static EntityInfo() {
			_codepointsByEntityName = new Dictionary<string, int[]>();
			_entityNameByCodepoint = new Dictionary<int, string>();

			// EntityInfo.bin follows a very simple format designed to be easy to read.
			// Each entity is defined in EntityInfo.bin as:
			// 
			// 1. 8 bits denoting the number of characters in the entity name
			// 2. A sequence of 8 bit ASCII characters, the entity name
			// 3. 8 bits denoting the number of codepoints in the entity value
			// 4. A sequence of 32 bit UTF32 codepoints, the entity value
			var readBuffer = new byte[64];
			var embeddedResourceName = typeof(EntityInfo).Namespace + ".EntityInfo.bin";
			using(var stream = typeof(EntityInfo).Assembly.GetManifestResourceStream(embeddedResourceName))
			do {
				// Read the number of 8-bit characters in the entity name
				var entityNameLength = stream.ReadByte();
				if(-1 == entityNameLength) break;

				// Read the entity's name
				stream.Read(readBuffer, 0, entityNameLength);
				var entityName = new string(readBuffer.Take(entityNameLength).Select(b => (char)b).ToArray());

				// Get the number of codepoints for the entity
				var numCodepoints = stream.ReadByte();

				// Read 32-bit UTF32 codepoints into an array
				var entityCodepoints = new int[numCodepoints];
				for(var i = 0; i < numCodepoints; i++) {
					stream.Read(readBuffer, 0, 4);
					entityCodepoints[i] = (readBuffer[0] << 24) | (readBuffer[1] << 16) | (readBuffer[2] << 8) | readBuffer[3];
				}

				_codepointsByEntityName.Add(entityName, entityCodepoints);
				if(1 == numCodepoints)
					_entityNameByCodepoint[entityCodepoints[0]] = entityName;
			} while(true);
		}

		public static IEnumerable<string> EntityNames { get { return _codepointsByEntityName.Keys; } }

		/// <summary>
		/// Returns true if the provided <paramref name="name"/> is a valid entity name.
		/// </summary>
		public static bool IsEntityName(string name) {
			return _codepointsByEntityName.ContainsKey(name);
		}

		public static bool TryGetEntityName(int codepoint, out string entityName) {
			return _entityNameByCodepoint.TryGetValue(codepoint, out entityName);
		}

		public static bool TryGetEntityValue(string entityName, out string value) {
			int[] codepoints;
			if(!_codepointsByEntityName.TryGetValue(entityName, out codepoints)) {
				value = null;
				return false;
			}

			value = 1 == codepoints.Length
				? char.ConvertFromUtf32(codepoints[0])
				: codepoints.Select(char.ConvertFromUtf32).JoinStrings();

			return true;
		}

		public static string GetEntityValue(string entityName) {
			var codepoints = _codepointsByEntityName[entityName];

			return 1 == codepoints.Length
				? char.ConvertFromUtf32(codepoints[0])
				: codepoints.Select(char.ConvertFromUtf32).JoinStrings();
		}
	}
}
