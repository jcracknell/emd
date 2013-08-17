using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
  public static class SuccessfulMatchingResult {
    public static SuccessfulMatchingResult<TProduct> Create<TProduct>(TProduct product, int length) {
      return new SuccessfulMatchingResult<TProduct>(product, length);
    }
  }

  public class SuccessfulMatchingResult<TProduct> : IMatchingResult<TProduct> {
    private readonly TProduct _product;
    private readonly int _length;

    public SuccessfulMatchingResult(TProduct product, int length) {
      _product = product;
      _length = length;
    }

    public bool Succeeded { get { return true; } }

    public int Length { get { return _length; } }

    public TProduct Product { get { return _product; } }

    object IMatchingResult.Product { get { return _product; } }
  }
}
