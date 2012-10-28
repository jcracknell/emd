﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public interface IExpressionNamingConvention {
		string Apply(string name, IParsingExpression expression);
	}
}
