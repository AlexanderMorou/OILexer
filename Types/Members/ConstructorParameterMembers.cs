using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A series of constructor parameter members.
    /// </summary>
    [Serializable]
    public class ConstructorParameterMembers :
        ParameteredParameterMembers<IConstructorParameterMember, CodeConstructor, IMemberParentType>,
        IConstructorParameterMembers
    {

        #region ConstructorParameterMembers Constructors
        public ConstructorParameterMembers(IConstructorMember targetDeclaration)
            : base(targetDeclaration)
        {
        }
        protected ConstructorParameterMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        protected override IMembers<IConstructorParameterMember, IParameteredDeclaration<IConstructorParameterMember, CodeConstructor, IMemberParentType>, CodeParameterDeclarationExpression> OnGetPartialClone(IParameteredDeclaration<IConstructorParameterMember, CodeConstructor, IMemberParentType> parent)
        {
            throw new NotSupportedException("Constructor parameters cannot be spanned across multiple instances as constructors are not segmentable.");
        }

        protected override IConstructorParameterMember GetParameterMember(TypedName data)
        {
            return new ConstructorParameterMember(data, (IConstructorMember)this.TargetDeclaration);
        }

    }
}
