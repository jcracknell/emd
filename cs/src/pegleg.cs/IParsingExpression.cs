using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs {
  public interface IParsingExpression {
    int Id { get; }
    T HandleWith<T>(IParsingExpressionHandler<T> handler);
    IMatchingResult Matches(MatchingContext context);
  }

  public interface IParsingExpression<out TProduct> : IParsingExpression {
    new IMatchingResult<TProduct> Matches(MatchingContext context);
  }
}
