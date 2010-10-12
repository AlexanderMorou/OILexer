using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    public class ClassTypes :
        DeclaredTypes<IClassType, CodeTypeDeclaration>,
        IClassTypes
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClassTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        public ClassTypes(ITypeParent parent) :
            base(parent)
        {
        }

        public ClassTypes(ITypeParent parent, ClassTypes sibling) :
            base(parent, sibling)
        {
            
        }

        /// <summary>
        /// Adds a new instance of the <see cref="IClassType"/> to the <see cref="ClassTypes"/>, given
        /// the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="IClassType"/>.</param>
        /// <returns>A new instance of <see cref="IClassType"/> if successful.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="name"/> is null.</exception>
        public override IClassType AddNew(string name)
        {
            return this.AddNew(name, new TypeConstrainedName[0]);
        }


        public override IClassType AddNew(string name, params TypeConstrainedName[] typeParameters)
        {
            IClassType ict = new ClassType(name, this.TargetDeclaration);

            foreach (TypeConstrainedName tcn in typeParameters)
                ict.TypeParameters.AddNew(tcn.Name, tcn.TypeReferences, tcn.RequiresConstructor);
            this._Add(ict.GetUniqueIdentifier(), ict);
            return ict;
        }

        #region IDeclarations<IClassType> Members

        IDeclarationTarget IDeclarations<IClassType>.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        #endregion

        protected override IDeclaredTypes<IClassType, CodeTypeDeclaration> OnGetPartialClone(ITypeParent partialTarget)
        {
            return this.GetPartialClone((ITypeParent)partialTarget);
        }

        #region IClassTypes Members

        public IClassTypes GetPartialClone(ITypeParent partialTarget)
        {
            return new ClassTypes(partialTarget, this);
        }

        /// <summary>
        /// Removes a class from the <see cref="ClassTypes"/> with the instance of the <see cref="IClassType"/>
        /// to remove provided.
        /// </summary>
        /// <param name="class">The class to be removed.</param>
        public void Remove(IClassType @class)
        {
            if (this.ContainsKey(@class.GetUniqueIdentifier()) && this[@class.GetUniqueIdentifier()] == @class)
                this.Remove(@class.GetUniqueIdentifier());
        }

        #endregion
    }
}
