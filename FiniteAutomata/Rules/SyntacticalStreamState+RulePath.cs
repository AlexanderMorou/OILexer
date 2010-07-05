using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        internal partial class RulePath :
            StatePath
        {
            #region RulePath data members
            private List<FollowInfo> followSet = new List<FollowInfo>();

            private StatePath parent;
            #endregion // RulePath data members
            #region RulePath properties
            public List<FollowInfo> FollowSet
            {
                get
                {
                    return this.followSet;
                }
            }

            public override bool IsRule
            {
                get
                {
                    return true;
                }
            }

            public override RulePath Rule
            {
                get
                {
                    return this;
                }
            }

            public new SyntacticalDFARootState Original
            {
                get
                {
                    return ((SyntacticalDFARootState)(base.Original));
                }
            }

            public StatePath Parent
            {
                get
                {
                    return this.parent;
                }
            }
            #endregion // RulePath properties
            #region RulePath methods
            public bool Equals(RulePath other)
            {
                if (!(base.Equals(other)))
                    return false;
                if (this.parent == null)
                    if (other.parent != null)
                        return false;
                    else
                        return true;
                return this.parent.Equals(other.parent);
            }

            public override bool Equals(StatePath other)
            {
                if (other == null)
                    return false;
                if (!(other.IsRule))
                    return false;
                return this.Equals(((RulePath)(other)));
            }

            public bool LineContains(IProductionRuleEntry target, RulePath followOwner, SyntacticalDFAState follower, StatePath parent)
            {
                /* ------------------------------------------------------\
                |  Line contains is a specialized check to determine if  |
                |  left recursion exists on the current rule.            |
                |--------------------------------------------------------|
                |  With exception to initial states being a terminal     |
                |  edge; thus, resulting in possible hidden left         |
                |  recursion, this should always be called on rules      |
                |  only.                                                 |
                \------------------------------------------------------ */
                StatePath currentState = this;
                FollowInfo followData = null;
                for (RulePath currentRule = this; (currentRule != null); )
                {
                    if (currentRule.Original.Entry == target)
                    {
                        if (followData == null)
                            followData = new FollowInfo(followOwner, follower);
                        if (!(currentRule.FollowSet.Contains(followData)))
                            currentRule.FollowSet.Add(followData);
                        return true;
                    }
                    currentState = currentRule.Parent;
                    if ((currentState != null) && currentState.IsRule)
                    {
                        /* -----------------------------------------------\
                        |  IsRule property due to lack of 'is' statement  |
                        |  in generator.                                  |
                        \----------------------------------------------- */
                        currentRule = ((RulePath)(currentState));
                    }
                    else
                        return false;
                }
                return false;
            }
            #endregion // RulePath methods
            #region RulePath .ctors
            public RulePath(SyntacticalDFARootState original)
                : base(original)
            {
            }

            public RulePath(SyntacticalDFARootState original, StatePath parent, RulePath followParent, SyntacticalDFAState follow)
                : this(original)
            {
                this.parent = parent;
                this.followSet.Add(new FollowInfo(followParent, follow));
            }
            #endregion // RulePath .ctors

            public override string ToString()
            {
                if (this.parent == null)
                    return this.Original.Entry.Name;
                else
                    return string.Format("{0}::{1}", this.Parent, this.Original.Entry.Name);
            }
        }
    }
}
