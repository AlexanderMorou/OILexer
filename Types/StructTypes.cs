using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types
{
    [Serializable]
    internal class StructTypes :
        DeclaredTypes<IStructType, CodeTypeDeclaration>,
        IStructTypes
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        internal StructTypes(ITypeParent targetDeclaration) 
            : base(targetDeclaration)
        {
        }
        internal StructTypes(ITypeParent partialTarget, IDictionary<string, IStructType> basePartialMembers)
            : base(partialTarget, basePartialMembers)
        {
        }
        protected StructTypes(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Adds a new instance of the <see cref="IStructType"/> to the <see cref="StructTypes"/>, given
        /// the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="IStructType"/>.</param>
        /// <returns>A new instance of <see cref="IStructType"/> if successful.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="name"/> is null.</exception>
        public override IStructType AddNew(string name)
        {
            return this.AddNew(name, new TypeConstrainedName[0]);
        }


        public override IStructType AddNew(string name, params TypeConstrainedName[] typeParameters)
        {
            IStructType ist = new StructType(name, this.TargetDeclaration);
            foreach (TypeConstrainedName tcn in typeParameters)
                ist.TypeParameters.AddNew(tcn.Name, tcn.TypeReferences, tcn.RequiresConstructor);
            base.Add(ist.GetUniqueIdentifier(), ist);
            return ist;
        }

        #region IDeclarations<IStructType> Members

        IDeclarationTarget IDeclarations<IStructType>.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        #endregion


        protected override IDeclaredTypes<IStructType, CodeTypeDeclaration> OnGetPartialClone(ITypeParent partialTarget)
        {
            return this.GetPartialClone(partialTarget);
        }

        #region IStructTypes Members

        public IStructTypes GetPartialClone(ITypeParent partialTarget)
        {
            return new StructTypes(partialTarget, this.dictionaryCopy);
        }

        #endregion

    }
}
