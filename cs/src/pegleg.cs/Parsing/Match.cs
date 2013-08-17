using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
  public class Match<TProduct> : IMatch<TProduct> {
    private readonly MatchingContext _context;
    private readonly TProduct _product;
    private readonly int _index;
    private readonly int _length;
    private readonly int _sourceIndex;
    private readonly int _sourceLength;
    private readonly int _sourceLine;
    private readonly int _sourceLineIndex;
    private SourceRange _sourceRange = null;

    public Match(MatchingContext context, TProduct product, int index, int length, int sourceIndex, int sourceLength, int sourceLine, int sourceLineIndex) {
      _context = context;
      _product = product;
      _index = index;
      _length = length;
      _sourceIndex = sourceIndex;
      _sourceLength = sourceLength;
      _sourceLine = sourceLine;
      _sourceLineIndex = sourceLineIndex;
    }

    public SourceRange SourceRange { 
      get { return _sourceRange ?? (_sourceRange = new SourceRange(_sourceIndex, _sourceLength, _sourceLine, _sourceLineIndex)); }
    }

    public int Length { get { return _length; } }

    public TProduct Product { get { return _product; } }

    public string String { get { return _context.Substring(_index, _length); } }
  }
}
