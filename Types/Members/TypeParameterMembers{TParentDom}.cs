using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{

    /// <summary>
    /// A type-strict dictionary of named <see cref="ITypeParameterMember{TParentDom}"/>s.
    /// </summary>
    /// <typeparam name="TParentDom">The <see cref="CodeTypeDeclaration"/> the parent
    /// instances of <typeparamref name="TItem"/> export.</typeparam>
    [Serializable]
    public class TypeParameterMembers<TParentDom> :
        TypeParameterMembers<ITypeParameterMember<TParentDom>, CodeTypeParameter, IDeclaredType<TParentDom>>,
        ITypeParameterMembers<ITypeParameterMember<TParentDom>, TParentDom>
        where TParentDom :
            CodeTypeDeclaration
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeParameterMembers{TParentDom}"/>
        /// with the <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IDeclaredType{TParentDom}"/> that
        /// contains the <see cref="TypeParameterMembers{TParentDom}"/>.</param>
        public TypeParameterMembers(IDeclaredType<TParentDom> targetDeclaration)
            : base(targetDeclaration)
        {
        }
        protected TypeParameterMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TypeParameterMembers(IDeclaredType<TParentDom> targetDeclaration, IDictionary<string, ITypeParameterMember<TParentDom>> partialBaseMembers)
            : base(targetDeclaration, partialBaseMembers)
        {
        }
        #region TypeParameterMembers<ITypeParameterMember<TParentDom>,TParentDom> Members

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name)
        {
            return this.AddNew(name, TypeParameterSpecialCondition.None);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> 
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, bool requiresConstructor)
        {
            return this.AddNew(name, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> and 
        /// <paramref name="constraints"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// constraints as expressed by <paramref name="constraints"/>..</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReferenceCollection constraints)
        {
            return this.AddNew(name, constraints, TypeParameterSpecialCondition.None);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/>, <paramref name="constraints"/>,
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>
        /// and additional constraints as expressed by <paramref name="constraints"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor)
        {
            return this.AddNew(name, constraints, requiresConstructor, TypeParameterSpecialCondition.None);
        }


        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReference[] constraints, bool requiresConstructor)
        {
            return this.AddNew(new TypeConstrainedName(name, requiresConstructor, TypeParameterSpecialCondition.None, constraints));
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, false, specialCondition);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> 
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, new TypeReferenceCollection(), requiresConstructor, specialCondition);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/> and 
        /// <paramref name="constraints"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// constraints as expressed by <paramref name="constraints"/>..</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReferenceCollection constraints, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, constraints, false, specialCondition);
        }

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/> with the <paramref name="name"/>, <paramref name="constraints"/>,
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>
        /// and additional constraints as expressed by <paramref name="constraints"/>.</returns>
        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            TypeParameterMember<TParentDom> item = new TypeParameterMember<TParentDom>(name, this.TargetDeclaration);
            item.RequiresConstructor = requiresConstructor;
            item.Constraints.AddRange(constraints.ToArray());
            item.SpecialCondition = specialCondition;
            this.Add(item.Name, item);
            return item;
        }


        public override ITypeParameterMember<TParentDom> AddNew(string name, ITypeReference[] constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(new TypeConstrainedName(name, requiresConstructor, specialCondition, constraints));
        }

        public override ITypeParameterMember<TParentDom> AddNew(TypeConstrainedName data)
        {
            return AddNew(data.Name, new TypeReferenceCollection(data.TypeReferences), data.RequiresConstructor, data.SpecialCondition);
        }

        public new void Clear()
        {
            base.Clear();
        }

        public override void Remove(string name)
        {
            base.Remove(name);
        }
        #endregion
        public ITypeParameterMembers<ITypeParameterMember<TParentDom>, TParentDom> GetPartialClone(IDeclaredType parent)
        {
            if (!(parent is IDeclaredType<TParentDom>))
                throw new ArgumentException("must be of type IDeclaredType<TParentDom>", "parent");
            return new TypeParameterMembers<TParentDom>((IDeclaredType<TParentDom>)parent, this.dictionaryCopy);
        }

        protected override IMembers<ITypeParameterMember<TParentDom>, IDeclaredType<TParentDom>, CodeTypeParameter> OnGetPartialClone(IDeclaredType<TParentDom> parent)
        {
            return this.GetPartialClone(parent);
        }
    }
}
