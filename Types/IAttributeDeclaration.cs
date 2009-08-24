using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with an attribute declaration on a type/member.
    /// </summary>
    public interface IAttributeDeclaration :
        ITypeReferenceable
    {
        string Name { get; set; }
        ITypeReference AttributeType { get; set; }
        IAttributeConstructorParameters Parameters { get; }
        CodeAttributeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options);
    }
}
