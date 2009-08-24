using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a constructor argument on an
    /// attribute declaration of a type/member.
    /// </summary>
    public interface IAttributeConstructorParameter :
        ITypeReferenceable,
        IDisposable
    {
        /// <summary>
        /// Returns/sets the value of the <see cref="IAttributeConstructorParameter"/>.
        /// </summary>
        IExpression Value { get; set; }

        CodeAttributeArgument GenerateCodeDom(ICodeDOMTranslationOptions options);

    }
}
