﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
  public abstract class AheadParsingExpression<TBody, TProduct> : BaseParsingExpression<TProduct> {
    protected readonly IParsingExpression<TBody> _body;

    public AheadParsingExpression(IParsingExpression<TBody> body)
    {
      if(null == body) throw Xception.Because.ArgumentNull(() => body);

      _body = body;
    }

    public IParsingExpression<TBody> Body { get { return _body; } }

    public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }

  public class NonCapturingAheadParsingExpression<TBody> : AheadParsingExpression<TBody, TBody> {
    public NonCapturingAheadParsingExpression(IParsingExpression<TBody> body)
      : base(body)
    { }

    public override IMatchingResult<TBody> Matches(MatchingContext context) {
      return _body.Matches(context.Clone());
    }
  }

  public class CapturingAheadParsingExpression<TBody, TProduct> : AheadParsingExpression<TBody, TProduct> {
    private readonly Func<IMatch<TBody>, TProduct> _matchAction;

    public CapturingAheadParsingExpression(IParsingExpression<TBody> body, Func<IMatch<TBody>, TProduct> matchAction)
      : base(body)
    {
      if(null == matchAction) throw Xception.Because.ArgumentNull(() => matchAction);
      
      _matchAction = matchAction;
    }

    public override IMatchingResult<TProduct> Matches(MatchingContext context) {
      var bodyMatchingContext = context.Clone();
      var bodyMatchingResult = _body.Matches(bodyMatchingContext);

      if(bodyMatchingResult.Succeeded)
        return context.GetMatchBuilderFor(this).Success(bodyMatchingResult.Product, _matchAction);

      return UnsuccessfulMatchingResult.Create(this);
    }
  }
}
