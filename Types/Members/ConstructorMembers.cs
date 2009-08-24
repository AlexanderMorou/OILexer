using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class ConstructorMembers :
        Members<IConstructorMember, IMemberParentType, CodeConstructor>,
        IConstructorMembers
    {

        public ConstructorMembers(IMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {

        }
        public ConstructorMembers(IMemberParentType targetDeclaration, IDictionary<string, IConstructorMember> partialBaseMembers)
            : base(targetDeclaration, partialBaseMembers)
        {

        }

        protected override IMembers<IConstructorMember, IMemberParentType, CodeConstructor> OnGetPartialClone(IMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #region IConstructorMembers Members

        public new IConstructorMembers GetPartialClone(IMemberParentType parent)
        {
            return new ConstructorMembers(parent, this.dictionaryCopy);
        }

        public IConstructorMember AddNew(IExpressionCollection cascadeMembers, ConstructorCascadeTarget cascadeExpressionsTarget, params TypedName[] parameters)
        {
            IConstructorMember result = new ConstructorMember(this.TargetDeclaration);
            foreach (TypedName param in parameters)
                result.Parameters.AddNew(param);
            if ((cascadeExpressionsTarget != ConstructorCascadeTarget.Undefined) && (cascadeMembers != null))
                foreach (IExpression cascParam in cascadeMembers)
                    result.CascadeMembers.Add(cascParam);
            result.CascadeExpressionsTarget = cascadeExpressionsTarget;
            this.Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        public IConstructorMember AddNew(params TypedName[] parameters)
        {
            return this.AddNew(null, ConstructorCascadeTarget.Undefined, parameters);
        }

        public new void Add(IConstructorMember constructor)
        {
            this.dictionaryCopy.Add(constructor.GetUniqueIdentifier(), constructor);
        }
        #endregion

    }
}
