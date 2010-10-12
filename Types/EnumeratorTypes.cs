using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    public class EnumeratorTypes :
        DeclaredTypes<IEnumeratorType, CodeTypeDeclaration>,
        IEnumeratorTypes
    {
        public EnumeratorTypes(ITypeParent targetDeclaration)
            : base(targetDeclaration)
        {
        }

        public EnumeratorTypes(ITypeParent targetDeclaration, EnumeratorTypes sibling)
            : base(targetDeclaration, sibling)
        {
        }
        public override IEnumeratorType AddNew(string name)
        {
            IEnumeratorType ietResult = new EnumeratorType(name, this.TargetDeclaration);
            this.Add(ietResult);
            return ietResult;
        }

        public override IEnumeratorType AddNew(string name, params TypeConstrainedName[] typeParameters)
        {
            throw new NotSupportedException("Enumerators aren't generic.");
        }

        protected override IDeclaredTypes<IEnumeratorType, CodeTypeDeclaration> OnGetPartialClone(ITypeParent partialTarget)
        {
            return this.GetPartialClone(partialTarget);
        }


        #region IEnumeratorTypes Members

        public IEnumeratorTypes GetPartialClone(ITypeParent partialTarget)
        {
            return new EnumeratorTypes(partialTarget, this);
        }

        #endregion

        #region IEnumeratorTypes Members


        public void Remove(IEnumeratorType @enum)
        {
            if (this.ContainsKey(@enum.GetUniqueIdentifier()))
                base.Remove(@enum.GetUniqueIdentifier());
        }

        #endregion
    }
}
