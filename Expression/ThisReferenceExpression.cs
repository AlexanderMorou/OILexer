using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// Provides an expression that refers to a <see cref="ITypeParent"/>.
    /// </summary>
    [Serializable]
    public class ThisReferenceExpression :
        MemberParentExpression<CodeThisReferenceExpression>,
        IThisReferenceExpression
    {

        /// <summary>
        /// Performs a look-up on the <see cref="ThisReferenceExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ThisReferenceExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        /// <remarks><see cref="ThisReferenceExpression"/> carries no references.</remarks>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            return;
        }
    }
}
