﻿using markdom.cs.Expressions;
using markdom.cs.Nodes;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class RuleTests : GrammarTestFixture {
		[Fact]
		public void Bla() {
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			var match = Grammar.Document.ShouldMatch(
				"# Heading 1\n",
				"\n",
				"@toc({style:float-right})\n",
				"\n",
				"This is a [paragraph] with **bold text**.\n",
				"\n",
				"## Heading 2\n",
				"Another @code({lang:'js'}, `\n",
				"(function(){\n",
				"	alert('bink!');\n",
				"})()\n",
				"`).\n",
				"\n",
				" * This is a tight list.\n",
				" * With two items, one of which is spread\n",
				"across several lines.\n",
				"\n",
				"Yet another @{{*emphasis*}}.\n",
				"\n",
				"|2c= The Title |\n",
				"|=   a  |=  b  |\n",
				"|     1 |    2 |\n",
				"|@{{\n",
				"	| foo | bar\n",
				"	| @{{**strong**}} | baz\n",
				"}}\n",
				"\n",
				"> blockquote\n",
				"with a second line\n",
				"\n",
				">and a second paragraph\n",
				"\n",
				"* Paragraph\n",
				"\n",
				"	> blockquote\n",
				"\n",
				"	> blockquote continues\n",
				"\n",
				"[slashdot.org]: http://slashdot.org\n",
				"[google.com]: (http://google.com, {'title': 'Google home page'})\n",
				"\n",
				"1. Item 1.\n",
				"a@42) Item 2.\n",
				"XVII - Roman.\n",
				"\n",
				"* Loose item 1.\n",
				"* Loose item 2.\n",
				"\n",
				"* Loose item 3.\n",
				"\n",
				"    With a second paragraph.\n"
			);
			stopwatch.Stop();

			var document = match.Product as MarkdomDocumentNode;

			var references = new markdom.cs.Conversion.ReferenceCollection(document);

			stopwatch.ToString();
		}
	}
}
