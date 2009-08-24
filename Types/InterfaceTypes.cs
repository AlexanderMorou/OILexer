using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    internal class InterfaceTypes :
        DeclaredTypes<IInterfaceType, CodeTypeDeclaration>,
        IInterfaceTypes
    {

        #region InterfaceTypes Constructors
        /// <summary>
        /// Creates a new instance of <see cref="InterfaceTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        internal InterfaceTypes(ITypeParent parent)
            :
            base(parent)
        {
        }

        internal InterfaceTypes(ITypeParent parent, IDictionary<string, IInterfaceType> basePartialMembers)
            :
            base(parent, basePartialMembers)
        {

        }
        #endregion

        /// <summary>
        /// Adds a new instance of the <see cref="IInterfaceType"/> to the <see cref="InterfaceTypes"/>, given
        /// the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="IInterfaceType"/>.</param>
        /// <returns>A new instance of <see cref="IInterfaceType"/> if successful.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="name"/> is null.</exception>
        public override IInterfaceType AddNew(string name)
        {
            return this.AddNew(name, new TypeConstrainedName[0]);
        }


        public override IInterfaceType AddNew(string name, params TypeConstrainedName[] typeParameters)
        {
            IInterfaceType ict = new InterfaceType(name, this.TargetDeclaration);
            foreach (TypeConstrainedName tcn in typeParameters)
                ict.TypeParameters.AddNew(tcn.Name, tcn.TypeReferences, tcn.RequiresConstructor);
            base.Add(ict.GetUniqueIdentifier(), ict);
            return ict;
        }

        protected override IDeclaredTypes<IInterfaceType, CodeTypeDeclaration> OnGetPartialClone(ITypeParent partialTarget)
        {
            return this.GetPartialClone(partialTarget);
        }

        #region IInterfaceTypes Members

        public IInterfaceTypes GetPartialClone(ITypeParent partialTarget)
        {
            return new InterfaceTypes(partialTarget, this.dictionaryCopy);
        }

        #endregion
    }
}
