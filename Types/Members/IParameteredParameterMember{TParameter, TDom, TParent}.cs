using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a generic-form method signature
    /// parameter.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameter.</typeparam>
    /// <typeparam name="TSignatureDom">The <see cref="CodeObject"/> the signature yields.</typeparam>
    /// <typeparam name="TParent">The type of parent which holds the method signatures.</typeparam>
    public interface IParameteredParameterMember<TParameter, TParameteredDom, TParent> :
        IMember<IParameteredDeclaration<TParameter, TParameteredDom, TParent>, CodeParameterDeclarationExpression>,
        IAutoCommentFragmentMember
        where TParameter :
            IParameteredParameterMember<TParameter, TParameteredDom, TParent>
        where TParent :
            IDeclarationTarget
        where TParameteredDom :
            CodeObject
    {
        /// <summary>
        /// Returns/sets the <see cref="ITypeReference"/> that the 
        /// <see cref="IParameteredParameterMember{TParameter, TSignatureDom, TParent}"/> accepts.
        /// </summary>
        ITypeReference ParameterType { get; set; }
        /// <summary>
        /// Returns/sets the direction the value of the <see cref="IParameteredParameterMember{TParameter, TSignatureDom, TParent}"/>
        /// will go.
        /// </summary>
        FieldDirection Direction { get; set; }
        new IVariableReferenceExpression GetReference();
    }
}
