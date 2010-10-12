using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public abstract class TypeParameterMembers<TItem, TDom, TParent> :
        Members<TItem, TParent, TDom>,
        ITypeParameterMembers<TItem, TDom, TParent>,
        ITypeParameterMembers
        where TItem :
            ITypeParameterMember<TDom, TParent>
        where TDom :
            CodeTypeParameter,
            new()
        where TParent :
            class,
            IDeclaration
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>
        /// with the <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <typeparamref name="TParent"/> that
        /// contains the <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>.</param>
        public TypeParameterMembers(TParent targetDeclaration)
            : base(targetDeclaration)
        {
        }
        public TypeParameterMembers(TParent targetDeclaration, TypeParameterMembers<TItem, TDom, TParent> sibling)
            : base(targetDeclaration, sibling)
        {
        }
        #region ITypeParameterMembers<TItem,TDom,TParent> Members

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/>.</returns>
        public abstract TItem AddNew(string name);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> 
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>.</returns>
        public abstract TItem AddNew(string name, bool requiresConstructor);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> and 
        /// <paramref name="constraints"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// constraints as expressed by <paramref name="constraints"/>..</returns>
        public abstract TItem AddNew(string name, ITypeReferenceCollection constraints);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="TypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/>, <paramref name="constraints"/>,
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>
        /// and additional constraints as expressed by <paramref name="constraints"/>.</returns>
        public abstract TItem AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor);

        /// <summary>
        /// Inserts a new <typeparam name="TItem"/> into the <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>
        /// with the <paramref name="name"/>, <paramref name="constraints"/>, and <paramref name="requiresConstructor"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new  <see cref="IMethodSignatureTypeParameterMember"/></param>
        /// <param name="constraints">The constraints of the new  <see cref="IMethodSignatureTypeParameterMember"/></param>
        /// <param name="requiresConstructor">Whether the new  <see cref="IMethodSignatureTypeParameterMember"/> 
        /// has a null-parameter constructor as a condition.</param>
        /// <returns>A new instance of the type-parameter created and inserted.</returns>
        public abstract TItem AddNew(string name, ITypeReference[] constraints, bool requiresConstructor);

        /// <summary>
        /// Inserts a new <typeparam name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the data provided.
        /// </summary>
        /// <param name="data">The information about the type-parameter.</param>
        /// <returns>A new instance of the type-parameter created and inserted.</returns>
        public abstract TItem AddNew(TypeConstrainedName data);

        #endregion

        #region ITypeParameterMembers Members

        public IDictionary<string, ITypeReference> GetTypeReferenceListing()
        {
            Dictionary<string, ITypeReference> result = new Dictionary<string, ITypeReference>();
            ITypeParameterMembers tParams = this;
            IDeclarationTarget target = this.TargetDeclaration;
            while (target != null)
            {
                foreach (ITypeParameterMember arg in tParams.Values)
                {
                    result.Add(arg.Name, arg.GetTypeReference());
                }
                if (target.ParentTarget != null)
                    target = target.ParentTarget;
                if (target is IParameteredDeclaredType)
                    tParams = ((IParameteredDeclaredType)target).TypeParameters;
                else
                    target = null;
            }
            return result;
        }

        #endregion

        #region IAutoCommentFragmentMembers Members

        public CodeCommentStatementCollection GenerateCommentCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeCommentStatementCollection ccsc = new CodeCommentStatementCollection();
            if (this.Count == 0)
                return ccsc;
            CodeCommentStatement[] typeParamComments = new CodeCommentStatement[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                TItem itpm = this[i];
                if (itpm.DocumentationComment != null && itpm.DocumentationComment != string.Empty)
                {
                    string itpmName = "";
                    if (options.NameHandler.HandlesName(itpm))
                        itpmName = options.NameHandler.HandleName(itpm);
                    else
                        itpmName = itpm.Name;
                    ccsc.Add(new CodeCommentStatement(new CodeComment(string.Format("<typeparam name=\"{0}\">{1}</typeparam>", itpmName, itpm.DocumentationComment), true)));
                }
            }
            return ccsc;
        }

        #endregion

        #region ITypeParameterMembers<TItem,TDom,TParent> Members

        public abstract TItem AddNew(string name, TypeParameterSpecialCondition specialCondition);

        public abstract TItem AddNew(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);

        public abstract TItem AddNew(string name, ITypeReferenceCollection constraints, TypeParameterSpecialCondition specialCondition);

        public abstract TItem AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);

        public abstract TItem AddNew(string name, ITypeReference[] constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);

        #endregion

    }
}
