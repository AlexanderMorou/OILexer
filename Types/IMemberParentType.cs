using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types.Members;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IDeclaredType"/>
    /// segmentable which may contain methods, properties, constructors, fields and 
    /// snippet members.
    /// </summary>
    public interface IMemberParentType :
        ISegmentableDeclaredType,
        IFieldParentType
    {
        /// <summary>
        /// Returns the methods defined on the <see cref="IMemberParentType"/>.
        /// </summary>
        IMethodMembers Methods { get; }
        /// <summary>
        /// Returns the properties defined on the <see cref="IMemberParentType"/>.
        /// </summary>
        IPropertyMembers Properties { get; }
        /// <summary>
        /// Returns the <see cref="IMemberParentType"/>'s constructors listing.
        /// </summary>
        IConstructorMembers Constructors { get; }
        /// <summary>
        /// Returns/sets members which are made up of a code-snippet.
        /// </summary>
        ISnippetMembers SnippetMembers { get; }
        /// <summary>
        /// Returns/sets members which coerce interpretations of expressions.
        /// </summary>
        IExpressionCoercionMembers Coercions { get; }
        /// <summary>
        /// Returns the fields defined on the <see cref="IFieldParentType"/>.
        /// </summary>
        new IFieldMembers Fields { get; }
        IThisReferenceExpression GetThisExpression();
        /// <summary>
        /// Returns the number of members contained within the <see cref="IMemberParentType"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> relating to the number of members within
        /// the <see cref="IMemberParentType"/></returns>
        new int GetMemberCount();
    }
}
