using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public interface IDeclaredTypes<TItem, TDom> :
        IDeclarations<TItem>
        where TItem :
            IDeclaredType<TDom>
        where TDom :
            CodeTypeDeclaration
    {

        /// <summary>
        /// Returns the <see cref="ITypeParent"/> the <see cref="IDeclaredTypes{TItem, TDom}"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeParent"/> that contains the <see cref="IDeclaredTypes{TItem, TDom}"/>.
        /// </returns>
        new ITypeParent TargetDeclaration { get; }

        /// <summary>
        /// Generates code-dom for the types declared.
        /// </summary>
        /// <param name="options">The options that determine scope and name management.</param>
        /// <returns>A series of <typeparamref name="TDom"/> pertinent to the elements
        /// of <see cref="IDeclaredTypes{TItem, TDom}"/></returns>
        TDom[] GenerateCodeDom(ICodeDOMTranslationOptions options);

        /// <summary>
        /// Creates a new instance of <typeparamref name="TItem"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new <typeparamref name="TItem"/>.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> if successful.</returns>
        TItem AddNew(string name);

        /// <summary>
        /// Inserts an already created instance.
        /// </summary>
        /// <param name="declaredType">The <typeparamref name="TITem"/> 
        /// to insert.</param>
        void Add(TItem declaredType);

        /// <summary>
        /// Creates a new instance of the <see cref="IDeclaredTypes{TItem, TDom}"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="partialTarget">The partial of the <see cref="TargetDeclaration"/> which
        /// needs a <see cref="IDeclaredTypes{TItem, TDom}"/> implementation instance.</param>
        /// <returns>A new <see cref="IDeclaredTypes{TItem, TDom}"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        IDeclaredTypes<TItem, TDom> GetPartialClone(ITypeParent partialTarget);
        string GetUniqueName(string name);
        /// <summary>
        /// Returns the number of types in the <see cref="IDeclaredTypes{TItem, TDom}"/> that target what's provided.
        /// </summary>
        /// <param name="target">The type parent to check which against the members.</param>
        /// <returns>An integer containing the number of <typeparamref name="TItem"/> instances that are children of the <paramref name="target"/>.</returns>
        int GetCountForTarget(ITypeParent target);
    }
}
