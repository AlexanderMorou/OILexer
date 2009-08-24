using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a series of expressions.
    /// </summary>
    public interface IExpressionCollection :
        IList<IExpression>,
        ITypeReferenceable
    {
        /// <summary>
        /// Generates a series of <see cref="CodeExpression"/>s which correlates to the series
        /// of <see cref="IExpression"/> elements in the <see cref="IExpressionCollection"/>.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A series of <see cref="CodeExpression"/>s pertinent to the <see cref="IExpressionCollection"/>
        /// entries.</returns>
        CodeExpression[] GenerateCodeDom(ICodeDOMTranslationOptions options);
    }
}
