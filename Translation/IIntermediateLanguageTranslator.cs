using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.Reflection.Emit;

namespace Oilexer.Translation
{
    public interface IIntermediateLanguageTranslator :
        IIntermediateTranslator
    {
        /// <summary>
        /// Returns the build lookup of the <see cref="IIntermediateLanguageTranslator"/>.
        /// </summary>
        /// <remarks>Each <see cref="IDeclarationTarget"/> relates to a <see cref="System.Reflection.Emit"/> member.</remarks>
        IDictionary<IDeclarationTarget, object> BuildLookup { get; }
        new AssemblyBuilder TranslateProject(IIntermediateProject project);
    }
}
