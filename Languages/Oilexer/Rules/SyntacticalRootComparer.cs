using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a syntactical root DFA state comparer.
    /// </summary>
    /// <remarks>Used to ensure that tree construction is minimized by ordering
    /// rules above the rules that directly depend upon them.</remarks>
    public class SyntacticalRootComparer :
        IComparer<SyntacticalDFARootState>
    {
        public static readonly SyntacticalRootComparer Singleton = new SyntacticalRootComparer();

        private SyntacticalRootComparer()
        {

        }
        #region IComparer<SyntacticalDFARootState> Members

        /// <summary>
        /// Compares two <see cref="SyntacticalDFARootState"/> instances
        /// based upon their dependency upon one another.
        /// </summary>
        /// <param name="x">The <see cref="SyntacticalDFARootState"/> to
        /// compare against <paramref name="y"/>.</param>
        /// <param name="y">The <see cref="SyntacticalDFARootState"/> to be
        /// compared to <paramref name="x"/>.</param>
        /// <returns>-1 if <paramref name="y"/> is dependant upon <paramref name="x"/>,
        /// 0 if <paramref name="x"/> and <paramref name="y"/> are identical, 
        /// 1 if <paramref name="x"/> is dependant upon <paramref name="y"/>; otherwise
        /// if <paramref name="x"/> and <paramref name="y"/> are cyclicly dependant
        /// or neither depend upon each other, the results are the comparison of their
        /// entry names.</returns>
        public int Compare(SyntacticalDFARootState x, SyntacticalDFARootState y)
        {
            var xRule = x.Entry;
            var yRule = y.Entry;
            if (xRule == yRule)
                return 0;
            bool xDependsOnY = x.DependsOn(yRule);
            bool yDependsOnX = y.DependsOn(xRule);
            if (xDependsOnY && !yDependsOnX)
                return 1;
            else if (yDependsOnX && !xDependsOnY)
                return -1;
            else
                return xRule.Name.CompareTo(yRule.Name);
        }

        #endregion

    }
}
