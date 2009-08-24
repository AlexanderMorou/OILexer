using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface IEnumeratorStatement :
        IBlockedStatement<CodeIterationStatement>,
        IBreakTargetStatement
    {
        /// <summary>
        /// Returns/sets the member reference which contains <see cref="IEnumerable.GetEnumerator()"/>.
        /// </summary>
        IMemberParentExpression EnumeratorSource { get; set; }
        /// <summary>
        /// Returns/sets the type of the items in the enumerator.
        /// </summary>
        ITypeReference ItemType { get; set; }
        /// <summary>
        /// The declared local relative to the enumeration.
        /// </summary>
        IStatementBlockLocalMember CurrentMember { get; }
    }
}
