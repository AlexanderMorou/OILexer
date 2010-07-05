using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState 
    {
        internal class SourcesInfo :
            IEnumerable<KeyValuePair<SourcedFrom, StatePath>>
        {
            #region SourcesInfo data members
            private StatePath[] initialSet;

            private StatePath[] firstSet;

            private StatePath[] followSet;
            #endregion // SourcesInfo data members
            #region SourcesInfo properties
            public StatePath[] InitialSet
            {
                get
                {
                    return this.initialSet;
                }
            }

            public StatePath[] FirstSet
            {
                get
                {
                    return this.firstSet;
                }
            }

            public StatePath[] FollowSet
            {
                get
                {
                    return this.followSet;
                }
            }

            public StatePath this[int index]
            {
                get
                {
                    int r1 = 0;
                    int r2 = this.initialSet.Length;
                    int r3 = (r2 + this.firstSet.Length);
                    int r4 = (r3 + this.followSet.Length);
                    if ((index >= r1) && (index < r2))
                        return this.initialSet[index];
                    else if ((index >= r2) && (index < r3))
                        return this.firstSet[(index - r2)];
                    else if ((index >= r3) && (index < r4))
                        return this.followSet[(index - r3)];
                    return null;
                }
            }

            public int Count
            {
                get
                {
                    return (this.initialSet.Length + (this.firstSet.Length + this.followSet.Length));
                }
            }
            #endregion // SourcesInfo properties
            #region SourcesInfo methods
            public IEnumerator<KeyValuePair<SourcedFrom, StatePath>> GetEnumerator()
            {
                foreach (StatePath initial in this.initialSet)
                {
                    yield return new KeyValuePair<SourcedFrom, StatePath>(SourcedFrom.Initial, initial);
                }
                foreach (StatePath first in this.firstSet)
                {
                    yield return new KeyValuePair<SourcedFrom, StatePath>(SourcedFrom.First, first);
                }
                foreach (StatePath follow in this.followSet)
                {
                    yield return new KeyValuePair<SourcedFrom, StatePath>(SourcedFrom.Follow, follow);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion // SourcesInfo methods
            #region SourcesInfo .ctors
            internal SourcesInfo(StatePath[] initialSet, StatePath[] firstSet, StatePath[] followSet)
            {
                this.initialSet = initialSet;
                this.firstSet = firstSet;
                this.followSet = followSet;
            }
            #endregion // SourcesInfo .ctors
        }

    }
}
