using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        internal class StatePath :
            IEquatable<StatePath>
        {
            #region StatePath data members
            private RulePath rule;

            private SyntacticalDFAState original;
            #endregion // StatePath data members
            #region StatePath properties
            public virtual bool IsRule
            {
                get
                {
                    return false;
                }
            }

            public virtual RulePath Rule
            {
                get
                {
                    return this.rule;
                }
            }

            public SyntacticalDFAState Original
            {
                get
                {
                    return this.original;
                }
            }
            #endregion // StatePath properties
            #region StatePath methods
            public virtual bool Equals(StatePath other)
            {
                if (other == null)
                    return false;
                if (object.ReferenceEquals(other, this))
                    return true;
                if (this.original != other.original)
                    return false;
                if (!((object.ReferenceEquals(this.rule, null) || this.Rule.Equals(other.Rule))))
                    return false;
                return true;
            }
            #endregion // StatePath methods
            #region StatePath .ctors
            public StatePath(SyntacticalDFAState original)
            {
                this.original = original;
            }

            public StatePath(SyntacticalDFAState original, RulePath rule)
                : this(original)
            {
                this.rule = rule;
            }
            #endregion // StatePath .ctors

            public override string ToString()
            {
                return this.Rule.ToString() + "::" + this.original.StateValue;
            }
        }

    }
}
