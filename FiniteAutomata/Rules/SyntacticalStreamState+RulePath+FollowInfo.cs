using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        partial class RulePath
        {
            internal class FollowInfo
            {
                #region FollowInfo data members
                private RulePath rule;

                private SyntacticalDFAState follow;
                #endregion // FollowInfo data members
                #region FollowInfo properties
                /// <summary>
                /// Returns the path to the rule which originally spawned the rule owning the current <see cref="FollowInfo"/>.
                /// </summary>
                public RulePath Rule
                {
                    get
                    {
                        return this.rule;
                    }
                }

                /// <summary>
                /// Returns the state to merge into the source set when the rule hits a terminal edge.
                /// </summary>
                public SyntacticalDFAState Follow
                {
                    get
                    {
                        return this.follow;
                    }
                }
                #endregion // FollowInfo properties
                #region FollowInfo methods
                public bool Equals(FollowInfo other)
                {
                    if (!(this.rule.Equals(other.rule)))
                        return false;
                    if (this.follow != other.follow)
                        return false;
                    return true;
                }

                public override bool Equals(object other)
                {
                    if (other == null)
                        return false;
                    if (other.GetType().IsSubclassOf(typeof(FollowInfo)))
                        return this.Equals(((FollowInfo)(other)));
                    return false;
                }

                public override int GetHashCode()
                {
                    int result = this.rule.GetHashCode();
                    int followCode = this.follow.GetHashCode();
                    int intersection = ~((followCode & result));
                    return (intersection & (result | followCode));
                }
                #endregion // FollowInfo methods
                #region FollowInfo .ctors
                public FollowInfo(RulePath rule, SyntacticalDFAState follow)
                {
                    this.rule = rule;
                    this.follow = follow;
                }
                #endregion // FollowInfo .ctors
            }
        }
    }
}
