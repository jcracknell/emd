using markdom.cs.Expressions;
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
			var input = @" // This is a single line comment
# Heading 1
This is a [paragraph] with **bold text**.

## Heading 2

Another @paragraph.

 * This is a tight list.
 * With two items, one of which is spread
across several lines.

Yet another @{{*emphasis*}}.

+==============+
|2c= The Title |
+=======+======+
|=   a  |=  b  |
+=======+======+
|     1 |    2 |
+-------+------+
|@{{
	| foo | bar
	| @{{**strong**}} | baz
}}

> blockquote
with a second line

>and a second paragraph

* Paragraph

	> blockquote

	> blockquote continues

[slashdot.org]: http://slashdot.org
[google.com]: (http://google.com, {'title': 'Google home page'})

1. Item 1.
a@42) Item 2.
XVII - Roman.

* Loose item 1.
* Loose item 2.

* Loose item 3.

    With a second paragraph.";
			
			var stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Start();
			var match = Grammar.Document.ShouldMatch(input);
			stopwatch.Stop();

			var document = match.Product as MarkdomDocumentNode;

			var references = new markdom.cs.Conversion.ReferenceCollection(document);

			stopwatch.ToString();
		}
	}
}
