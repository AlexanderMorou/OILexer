using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Linq;

namespace Oilexer.Types
{
    [Serializable]
    partial class ExternType 
    {
        [Serializable]
        public class TypeReference :
            TypeReferenceBase,
            IExternTypeReference
        {

            public TypeReference(IExternType typeInstance)
                : base(typeInstance)
            {
            }
            public TypeReference(IExternType typeInstance, ITypeReferenceCollection typeParameters)
                : base(typeInstance, typeParameters)
            {
            }

            private TypeReference(IExternTypeReference arrayElementType, int arrayRank)
                : base(arrayElementType, arrayRank)
            {
            }

            public override ITypeReference MakeArray(int rank)
            {
                return new TypeReference(this, rank);
            }

            #region IExternTypeReference Members

            public new IExternType TypeInstance
            {
                get
                {
                    return (IExternType)base.TypeInstance;
                }
            }
            #endregion
        }
    }
}
