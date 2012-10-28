﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public enum NodeKind {
		AutoLink,
		Emphasis,
		Entity,
		Heading,
		Image,
		InlineExpression,
		LineBreak,
		Link,
		OrderedList,
		OrderedListItem,
		Paragraph,
		Quoted,
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
