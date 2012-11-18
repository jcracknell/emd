using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public static class ParsingExpressionIdGenerator {
		private static int _idGenerator = 0;
		private static object _idGenerationLock = new object();

		public static int GenerateId() {
			// I have no idea under what circumstances you would multithread expression construction (as expression ids
			// need only be unique to the grammar), but we don't really care about construction performance, so it is
			// better to be safe than sorry.
			lock(_idGenerationLock) {
				var id = _idGenerator;
				_idGenerator = int.MaxValue == _idGenerator ? int.MinValue : _idGenerator + 1;
				return id;
			}
		}
	}
}
