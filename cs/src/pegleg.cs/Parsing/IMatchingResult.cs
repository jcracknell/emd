﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IMatchingResult {
		bool Succeeded { get; }
		object Product { get; }
	}
}