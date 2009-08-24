using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a generic declaration.
    /// </summary>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="IDeclaration{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which the
    /// <see cref="IDeclaration{TParent, TDom}"/> yields.</typeparam>
    public interface IDeclaration<TParent, TDom> :
        IDeclaration
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        /// <summary>
        /// Generates the <typeparamref name="TDom"/> that represents the <see cref="IDeclaration{TParent, TDom}"/>.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A new instance of a <typeparamref name="TDom"/> if successful.-null- otherwise.</returns>
        new TDom GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Returns the parentTarget target for the current target.
        /// </summary>
        new TParent ParentTarget { get; set; }

    }
}
