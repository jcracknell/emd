using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions {
  public abstract class RegexParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
    protected readonly Regex _regex;

    public RegexParsingExpression(Regex regex) {
      if(null == regex) throw Xception.Because.ArgumentNull(() => regex);

      _regex = regex;
    }

    public Regex Regex { get { return _regex; } }

    public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  } 

  public class NonCapturingRegexParsingExpression : RegexParsingExpression<Nil> {
    public NonCapturingRegexParsingExpression(Regex regex) : base(regex) { }

    public override IMatchingResult<Nil> Matches(MatchingContext context) {
      Match regexMatch;
      if(context.ConsumesMatching(_regex, out regexMatch))
        return SuccessfulMatchingResult.Create(Nil.Value, regexMatch.Length);
      return UnsuccessfulMatchingResult.Create(this);
    }
  }

  public class CapturingRegexParsingExpression<TProduct> : RegexParsingExpression<TProduct> {
    private readonly Func<IMatch<Match>, TProduct> _matchAction;

    public CapturingRegexParsingExpression(Regex regex, Func<IMatch<Match>, TProduct> matchAction)
      : base(regex)
    {
      if(null == matchAction) throw Xception.Because.ArgumentNull(() => matchAction);

      _matchAction = matchAction;
    }

    public override IMatchingResult<TProduct> Matches(MatchingContext context) {
      var matchBuilder = context.GetMatchBuilderFor(this);

      Match regexMatch;
      if(!context.ConsumesMatching(_regex, out regexMatch))
        return UnsuccessfulMatchingResult.Create(this);

      return matchBuilder.Success(regexMatch, _matchAction);
    }
  }
}