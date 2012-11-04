using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public class TableCellSpanningInfo {
		public readonly int ColumnSpan;
		public readonly int RowSpan;

		public TableCellSpanningInfo(int columnSpan, int rowSpan) {
			ColumnSpan = columnSpan;
			RowSpan = rowSpan;
		}
	}
}
