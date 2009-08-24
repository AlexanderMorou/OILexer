using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public interface ITypeReferenceable
    {
        /// <summary>
        /// Performs a look-up on the <see cref="ITypeReferenceable"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ITypeReferenceable"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options);
    }
}
