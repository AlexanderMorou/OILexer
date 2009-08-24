using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class PropertySetValueReferenceExpression :
        MemberParentExpression<CodePropertySetValueReferenceExpression>,
        IPropertySetValueReferenceExpression
    {
        public static PropertySetValueReferenceExpression Reference = new PropertySetValueReferenceExpression();
        private PropertySetValueReferenceExpression()
        {

        }

        /// <summary>
        /// Performs a look-up on the <see cref="PropertySetValueReferenceExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="PropertySetValueReferenceExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        /// <remarks><see cref="PropertySetValueReferenceExpression"/> carries no implied
        /// reference.</remarks>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            return;
        }
    }
}
