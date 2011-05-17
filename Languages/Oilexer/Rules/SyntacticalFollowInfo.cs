using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a class for determining the state which continues the path of 
    /// a rule once it terminates.
    /// </summary>
    public class SyntacticalFollowInfo
    {
        /// <summary>
        /// Creates a new <see cref="SyntacticalFollowInfo"/> instance
        /// which designates the origin and point of continuation.
        /// </summary>
        /// <param name="origin">The <see cref="SyntacticalDFAState"/> which 
        /// designates the point of origin.</param>
        /// <param name="continues">The <see cref="SyntacticalDFAState"/> which
        /// designates the point to continue the path.</param>
        public SyntacticalFollowInfo(SyntacticalDFAState origin, SyntacticalDFAState continues)
        {
            this.Origin = origin;
            this.Continues = continues;
        }
        /// <summary>
        /// The origin state which called for the rule.
        /// </summary>
        public SyntacticalDFAState Origin { get; private set; }
        /// <summary>
        /// The destination state which continues the path.
        /// </summary>
        public SyntacticalDFAState Continues { get; private set; }
    }
}
