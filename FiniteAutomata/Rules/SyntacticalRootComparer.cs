using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal;
namespace Oilexer.FiniteAutomata.Rules
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
            var rX = x.Entry;
            var rY = y.Entry;
            if (rX == rY)
                return 0;
            bool xD = x.DependsOn(rY);
            bool yD = y.DependsOn(rX);
            if (xD && !yD)
                return 1;
            else if (yD && !xD)
                return -1;
            else
                return rX.Name.CompareTo(rY.Name);
        }

        #endregion

    }
}
