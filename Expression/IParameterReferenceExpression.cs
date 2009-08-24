using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Oilexer.Comments;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a reference expression that points back to 
    /// a parameter type variable.
    /// </summary>
    public interface IParameterReferenceExpression :
        IVariableReferenceExpression
    {
        /// <summary>
        /// Creates a reference particle for the <see cref="IParameterReferenceExpression"/>.
        /// </summary>
        /// <returns>A new <see cref="IParameterReferenceComment"/> implementation instance which
        /// refers to the active <see cref="IParameterReferenceExpression"/>'s
        /// <see cref="IMemberReferenceExpression.Reference"/>.</returns>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new IParameterReferenceComment GetReferenceParticle();
    }
}