using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a field.
    /// </summary>
    public interface IFieldMember :
        IMember<IFieldParentType, CodeMemberField>,
        IAutoCommentMember,
        IExportTableMember
        //,
        //IFauxableReliant<FieldInfo, Type>
    {
        new bool IsStatic { get; set; }
        IExpression InitializationExpression { get; set; }
        ITypeReference FieldType { get; set; }
        /// <summary>
        /// Returns/sets whether the field is a constant value.
        /// </summary>
        bool IsConstant { get; set; }
        /// <summary>
        /// Returns a reference expression pertinent to the <see cref="IFieldMember"/>.
        /// </summary>
        /// <returns>A <see cref="IFieldReferenceExpression"/> implmementation
        /// which relates back to the <see cref="IFieldMember"/>.</returns>
        new IFieldReferenceExpression GetReference();
    }
}
