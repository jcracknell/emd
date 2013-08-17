# EMD 

EMD stands for "expressive markdown". This project is an abandoned attempt at an advanced markdown implementation integrating Javascript expressions as a macro language for extensibility, and parsing to a document object model (DOM) using a parsing expression grammar (PEG).

## Status

I had originally thought that I would be embarking on a multi-year project heavily leveraging a Markdown implementation which:

  * parses to a strongly-typed, analysable DOM;
  * is extensible, allowing for future addition of user-configurable presentation elements; and
  * featured full unicode support.

Unfortunately the motivating project fell through, and I have recently discovered an interest in exploring development on non-Windows platfoms. Consequently **work on EMD has reached a standstill**. (The best software projects scratch an itch, and I am no longer itchy).

I made a surprising amount of progress in the implementation - the grammar is mostly complete, and the parsing library *generally* works. The [`test/`](/jcracknell/emd/tree/master/test) folder contains working input files and the associated expected output expressed using a Lisp-like tree syntax. The parser is currently capable of handling more complex documents; these are the set of test cases I have implemented and debugged.

In fact the list of outstanding 'big ticket' items is fairly short:

  * Debugging of, and implementation of test cases for the grammar.
  * Implementation of a Javascript renderer for the parsed DOM nodes.
  * Developing an abstraction layer for the integration of Javascript runtimes such as [IronJS](https://github.com/fholm/IronJS) or [Jurassic](https://jurassic.codeplex.com/).

Unfortunately these still represent an enormous amount of work. I have made my work to date on EMD available under the permissive MIT Licence in hopes that some part of the code (or even the ideas) may be useful to you.

## Features

  * **A strongly-typed, analysable, traversable and transformable document object model.** EMD features a full DOM which can be found in the [`emd.cs.Nodes`](/jcracknell/emd/tree/master/cs/src/emd.cs/Nodes) namespace.

  * **Recursive embedding of a modified subset of javascript as a customizable macro language.** The javascript subset is extended to include a syntax for EMD [heredocs](https://en.wikipedia.org/wiki/Here_document), which in turn allow embedding of javascript. There are some syntactical challenges related to the embedding of heredocs as a result of markdown's significant left margin. I had planned to add support for explicit heredoc margins using the `|` character:

    ```
    @image('some.png', { caption: {{
      | This is a caption for a *hypothetical* (yet likely) image macro which accepts an optional
      | EMD heredoc caption. Because the heredoc **is** markdown, it can contain multiple paragraphs.
      | 
      | You can switch back into javascript again if you so desire. @image('smile.png')
    }} });
    ```

  * **An attempt at simplification and consistency through unification of markdown and javascript features and idioms:** 

    * Links are special cases of function calls (`[link text]('http://github.com', { title: 'the title' })`).
    * Unquoted URIs are valid literals (`@image(http://abc.com/some.png)`)
    * `@` used to switch to Javascript or to precede identifiers.
    * URIs, heredocs and backtick-delimited code blocks (verbatim strings) are idiomatic Javascript values:
      * `http://google.com === 'http://google.com`
      * `{{**heredoc**}}.toString() === 'heredoc'`
      * `` `code` === 'code'``
    * [Footnote-style references](/jcracknell/emd/blob/master/cs/src/emd.cs/Grammar/EmdGrammar.Blocks.cs#L477) are stored argument lists which can be applied to a link's function call.
    * Javascript-style comments and escape sequences are valid everywhere. Named HTML entities are reformulated as javascript escape sequences of the form `\copy`/`\gt`/`\lt`.

  * **Full unicode support.** All grammar productions are implemented in terms of graphemes instead of characters.

EMD takes a strong position on certain [ambiguities](http://johnmacfarlane.net/babelmark2/faq.html) and contentious misfeatures present in the markdown specification:

  * A single newline does not start a new block, as this is the user's expectation (manually wrapped paragraphs allowed).
  * No indented code blocks. I like to characterize this syntax as 'easy to invoke by accident, yet difficult to use with purpose'.
  * No built-in image syntax (define an `@image(...)` macro instead).
  * No underscore syntax for `**strong**` and `*emphasized*` text.
  * No 'Setext-style' (underlined) headers, as they are hard to parse and purely ornamental.
  * No HTML, including HTML entities. Entities are inserted using the unified javascript-style escape sequences.

## Ancillary Code Artifacts

Any project of this scope will naturally excrete a number of code artifacts which are most likely of use outside of the original problem domain.  

In the case of EMD, the elephant in the room is undoubtedly the [pegleg.cs](/jcracknell/emd/blob/master/cs/src/pegleg.cs/ParsingExpressions.cs) parsing expression grammar toolkit, which provides a (semi) fluent API for declaring parsing expression grammars in C#.

A traditional parser generator such as [IronMeta](http://ironmeta.sourceforge.net) runs a tool against a domain-specific language specifying a grammar, generating methods for parsing each production in the grammar. pegleg.cs provides an API for declaring a grammar within your application in C#, assembling a parser using classes for each parsing expression at runtime. This results in slightly reduced performance, but also has a number of suprisingly compelling benefits - for a start the grammar is fully integrated in your existing workflow, and is also fully refactorable.

Parsing expressions in pegleg.cs are [strongly-typed](/jcracknell/emd/blob/master/cs/src/pegleg.cs/IParsingExpression.cs) - the result of each parsing expression is known, whether it is a string or your own custom data structure. This enables compile-time verification of expression composability. 

The provided API makes extensive use of C#'s type inferral, jumping through [hoops](/jcracknell/emd/blob/master/cs/src/pegleg.cs/Parsing/Expressions/SequenceProducts.cs#L28) to improve the author's quality of life. The grammar syntax is relatively concise, and the author's IDE can provide full code completion support and highlight type errors found by the compiler. Consider the parsing expression below for a signed integer-valued double, where the product type of the sign and integer expressions are known and inferred, permitting the wonderful concision of the sequence product:

```cs
public static readonly IParsingExpression<double>
SignedInteger = Sequence(
  Optional(
    ChoiceUnordered(
      Literal("+", match => 1d),
      Literal("-", match => -1d)
    ),
    match   => match.Product,
    noMatch => 1d
  ),
  AtLeast(1, Reference(() => Digit), match => double.Parse(match.String)),
  // Product inferred to be of type SequenceProduct<double, double>
  match => match.Product.Of1 * match.Product.Of2
);
```

As expressions are initialized at runtime, it is possible to perform dynamic expansion of parsing expressions which would be impossible with a traditional parser generator. EMD makes use of this in generating its grammar for verbatim (backtick) string literals (below) and more extensively in its grammar for lists of varying styles.

```cs
public static readonly IParsingExpression<string>
VerbatimStringLiteral = Named(() => VerbatimStringLiteral,
  ChoiceOrdered(
    // Range used to generate 16 different choices
    Enumerable.Range(1,16).Reverse().Select(i => "".PadRight(i, '`')).Select(ticks =>
      Sequence(
        Literal(ticks),
        AtLeast(0,
          Sequence(
            NotAhead(Literal(ticks)),
            Reference(() => UnicodeCharacter)
          ),
          match => match.String
        ),
        Literal(ticks),
        match => match.Product.Of2
      )
    )
  )
);
```

Other lesser code artifacts and features which may also be of interest are:

* [The `pegleg.cs.Unicode.GraphemeCriteria` API](/jcracknell/emd/blob/master/cs/src/pegleg.cs/Unicode/GraphemeCriteria.cs) for composable, high-performance matching of unicode graphemes and codepoints.
* [Auto-optimizing `ChoiceUnordered` expressions](/jcracknell/emd/blob/master/cs/src/pegleg.cs/Parsing/Expressions/UnorderedChoiceParsingExpression.cs#L50), which self-optimize based on the match frequency of choices.
* [The `pegleg.cs.Unicode.UnicodeUtils` class](/jcracknell/emd/blob/master/cs/src/pegleg.cs/Unicode/UnicodeUtils.cs#L98). This is something of a re-invention of the wheel, but if you need to get both the length and category of a grapheme in a single method call this will get it done (surprisingly this is not possible using `System.Globalization.CharUnicodeInfo`).

## Acknowledgements

EMD's parsing expression grammar for markdown is based heavily on John MacFarlane's pioneering work in this area; specifically his [peg-markdown](https://github.com/jgm/peg-markdown/) and [pandoc](http://johnmacfarlane.net/pandoc/) projects.

Similarly, my grammar for javascript owes a debt of gratitude to David Majda's [PEG for javascript](https://github.com/dmajda/pegjs/blob/master/examples/javascript.pegjs), seemingly implemented as a test case (!) for his [PEG.js parser generator for javascript](http://pegjs.majda.cz/).

