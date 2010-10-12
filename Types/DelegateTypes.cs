using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    public class DelegateTypes :
        DeclaredTypes<IDelegateType, CodeTypeDelegate>,
        IDelegateTypes
    {
        /// <summary>
        /// Creates a new instance of <see cref="DelegateTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        public DelegateTypes(ITypeParent parent)
            :
            base(parent)
        {
        }

        public DelegateTypes(ITypeParent parent, DelegateTypes sibling)
            : base(parent, sibling)
        {

        }

        /// <summary>
        /// Adds a new instance of the <see cref="IDelegateType"/> to the <see cref="DelegateTypes"/>, given
        /// the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDelegateType"/>.</param>
        /// <returns>A new instance of <see cref="IDelegateType"/> if successful.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="name"/> is null.</exception>
        public override IDelegateType AddNew(string name)
        {
            return this.AddNew(name, new TypeConstrainedName[0]);
        }


        public override IDelegateType AddNew(string name, params TypeConstrainedName[] typeParameters)
        {
            IDelegateType ict = new DelegateType(name, this.TargetDeclaration);
            foreach (TypeConstrainedName tcn in typeParameters)
                ict.TypeParameters.AddNew(tcn.Name, tcn.TypeReferences, tcn.RequiresConstructor);
            this._Add(ict.GetUniqueIdentifier(), ict);
            return ict;
        }

        #region IDeclarations<IDelegateType> Members

        IDeclarationTarget IDeclarations<IDelegateType>.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        #endregion

        protected override IDeclaredTypes<IDelegateType, CodeTypeDelegate> OnGetPartialClone(ITypeParent partialTarget)
        {
            return this.GetPartialClone(partialTarget);
        }


        #region IDelegateTypes Members

        public IDelegateTypes GetPartialClone(ITypeParent partialTarget)
        {
            return new DelegateTypes(partialTarget, this);
        }

        #endregion
    }
}
