using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    public enum UnaryOperations
    {
        /// <summary>
        /// Negates the numeric value.
        /// </summary>
        /// <remarks><para>C#: '-' Expression</para>
        /// <para>VB: '-' Expression</para></remarks>
        Negate,
        /// <summary>
        /// Ignored
        /// </summary>
        Plus,
        /// <summary>
        /// Indirection
        /// </summary>
        /// <remarks><para>C#: '*' Expression (unsafe only)</para>
        /// <para>VB: n/a.</para></remarks>
        Indirection,
        /// <summary>
        /// Obtain the address of.
        /// </summary>
        /// <remarks><para>C#: '&' Expression (unsafe only)</para>
        /// <para>VB: n/a.</para></remarks>
        AddressOf,
        /// <summary>
        /// Obtain the logical inversion of.
        /// </summary>
        /// <remarks><para>C#: '!' Expression</para>
        /// <para>VB: "Not" Expression</para></remarks>
        LogicalNot,
        /// <summary>
        /// Obtain the compliement of.
        /// </summary>
        /// <remarks><para>C#: '~' Expression</para>
        /// <para>VB: "Not" Expression</para></remarks>
        Compliment,
        /// <summary>
        /// Prefix increment.
        /// </summary>
        /// <remarks><para>C#: "++" Expression</para>
        /// <para>VB: n/a.</para></remarks>
        PrefixIncrement,
        /// <summary>
        /// Prefix decrement.
        /// </summary>
        /// <remarks><para>C#: "--" Expression</para>
        /// <para>VB: n/a.</para></remarks>
        PrefixDecrement,
        /// <summary>
        /// Size of type.
        /// </summary>
        /// <remarks><para>C#: "sizeof" '(' TypeIdentifier ')'</para></remarks>
        SizeOf
    }
    /// <summary>
    /// Defines properties and methods for working with a unary operation expression.
    /// </summary>
    public interface IUnaryOperationExpression :
        IExpression<CodeSnippetExpression>
    {
        /// <summary>
        /// Returns/sets the operation referred to by the <see cref="IUnaryOperationExpression"/>.
        /// </summary>
        UnaryOperations Operation { get; set; }
        /// <summary>
        /// Returns/sets the <see cref="IExpression"/> which has the <see cref="Operation"/> applied to it.
        /// </summary>
        IExpression TargetExpression { get; set; }
    }
}
