using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    public class FiniteAutomataTransitionNode<TCheck, TTarget> :
        IFiniteAutomataTransitionNode<TCheck, TTarget>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
    {
        #region IFiniteAutomataTransitionNode<TCheck,TTarget> Members

        public TCheck Check { get; internal set; }

        public TTarget Target { get; internal set; }

        #endregion
    }
}
