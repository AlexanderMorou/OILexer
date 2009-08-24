using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with an expression.
    /// </summary>
    public interface IExpression :
        ITypeReferenceable
    {   
        /// <summary>
        /// Generates the <see cref="CodeExpression"/> for the <see cref="IExpression"/>.
        /// </summary>
        /// <param name="options">The <see cref="ICodeDOMTranslationOptions"/> which guide the
        /// generation process.</param>
        /// <returns>A new <see cref="CodeExpression"/> which relates to the <see cref="IExpression"/>.</returns>
        CodeExpression GenerateCodeDom(ICodeDOMTranslationOptions options);
    }
}
