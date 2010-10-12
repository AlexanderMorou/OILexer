using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A series of fields members.
    /// </summary>
    [Serializable]
    public class FieldMembers :
        Members<IFieldMember, IFieldParentType, CodeMemberField>,
        IFieldMembers
    {
        public FieldMembers(IFieldParentType targetDeclaration)
            : base(targetDeclaration)
        {

        }
        public FieldMembers(IFieldParentType targetDeclaration, FieldMembers sibling)
            : base(targetDeclaration, sibling)
        {

        }
        protected override IMembers<IFieldMember, IFieldParentType, CodeMemberField> OnGetPartialClone(IFieldParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #region IFieldMembers Members

        public new IFieldMembers GetPartialClone(IFieldParentType parent)
        {
            if (this.TargetDeclaration is ISegmentableDeclaredType)
                return new FieldMembers(parent, this);
            else
                throw new NotSupportedException("The parent type is non-segmentable.");
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndType">The name and return-type of the <see cref="IFieldMember"/>.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(TypedName nameAndType)
        {
            return this.AddNew(nameAndType, null);
        }
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndType">The name and return-type of the <see cref="IFieldMember"/>.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(TypedName nameAndType, IExpression initializationExpression)
        {
            IFieldMember result = new FieldMember(nameAndType, this.TargetDeclaration);
            if (initializationExpression != null)
                result.InitializationExpression = initializationExpression;
            this._Add(result.Name, result);
            return result;
        }

        public IFieldMember AddNew(string name, bool initValue)
        {
            return this.AddNew(new TypedName(name, typeof(bool)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, byte initValue)
        {
            return this.AddNew(new TypedName(name, typeof(byte)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, sbyte initValue)
        {
            return this.AddNew(new TypedName(name, typeof(sbyte)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, char initValue)
        {
            return this.AddNew(new TypedName(name, typeof(char)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, string initValue)
        {
            return this.AddNew(new TypedName(name, typeof(string)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, uint initValue)
        {
            return this.AddNew(new TypedName(name, typeof(uint)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, int initValue)
        {
            return this.AddNew(new TypedName(name, typeof(int)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, ulong initValue)
        {
            return this.AddNew(new TypedName(name, typeof(ulong)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, long initValue)
        {
            return this.AddNew(new TypedName(name, typeof(long)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, float initValue)
        {
            return this.AddNew(new TypedName(name, typeof(float)), new Expression.PrimitiveExpression(initValue));
        }

        public IFieldMember AddNew(string name, double initValue)
        {
            return this.AddNew(new TypedName(name, typeof(double)), new Expression.PrimitiveExpression(initValue));
        }

        #endregion

    }
}
