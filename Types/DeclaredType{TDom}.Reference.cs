using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    partial class DeclaredType<TDom>
    {
        protected class Reference :
            TypeReferenceBase,
            IDeclaredTypeReference<TDom>,
            IDeclaredTypeReference
        {
            private Reference(IDeclaredTypeReference arrayElementType, int arrayRank)
                : base(arrayElementType, arrayRank)
            {
            }
            /// <summary>
            /// Creates a new instance of <see cref="DeclaredTypeReference{TDom}"/> with the <paramref name="typeReference"/> provided.
            /// </summary>
            /// <param name="typeReference">The <see cref="IDeclaredType{TDom}"/> the <see cref="DeclaredTypeReference{TDom}"/> represents</param>
            public Reference(IDeclaredType<TDom> typeReference)
                : base(typeReference)
            {

            }
            /// <summary>
            /// Creates a new instance of <see cref="DeclaredTypeReference{TDom}"/> with the <paramref name="typeReference"/> provided.
            /// </summary>
            /// <param name="typeReference">The <see cref="IDeclaredType{TDom}"/> the <see cref="DeclaredTypeReference{TDom}"/> represents</param>
            public Reference(IDeclaredType<TDom> typeReference, ITypeReferenceCollection typeParameters)
                : base(typeReference, typeParameters)
            {

            }

            public override ITypeReference MakeArray(int rank)
            {
                return new Reference(this, rank);
            }

            #region ITypeReference<IDeclaredType<TDom>> Members

            public IDeclaredType<TDom> TypeReference
            {
                get { return (IDeclaredType<TDom>)base.TypeInstance; }
            }

            #endregion

            #region IDeclaredTypeReference<TDom> Members

            IDeclaredType<TDom> IDeclaredTypeReference<TDom>.TypeInstance
            {
                get { return this.TypeReference; }
            }

            #endregion

            #region IDeclaredTypeReference Members

            IDeclaredType IDeclaredTypeReference.TypeInstance
            {
                get {
                    return this.TypeReference;
                }
            }

            #endregion

            
        }
    }
}
