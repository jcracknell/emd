using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public enum NodeKind {
		AutoLink,
		Blockquote,
		Document,
		Emphasis,
		Entity,
		Heading,
		InlineExpression,
		LineBreak,
		Link,
		OrderedList,
		OrderedListItem,
		Paragraph,
		Quoted,
		Reference,
		Section,
		Space,
		Strong,
		Symbol,
		Table,
		TableDataCell,
		TableHeaderCell,
		TableRow,
		Text,
		UnorderedList,
		UnorderedListItem
	}
}
