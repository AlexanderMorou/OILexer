using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    public class EnumTypeFieldMembers :
        Members<IFieldMember, IFieldParentType, CodeMemberField>,
        IEnumTypeFieldMembers
    {
        /// <summary>
        /// Data member for <see cref="EnumType"/>.
        /// </summary>
        private ITypeReference enumType;

        public EnumTypeFieldMembers(IEnumeratorType targetDeclaration)
            : base(targetDeclaration)
        {
            this.enumType = targetDeclaration.GetTypeReference();
        }
        protected override IMembers<IFieldMember, IFieldParentType, CodeMemberField> OnGetPartialClone(IFieldParentType parent)
        {
            throw new NotSupportedException("Enums cannot contain partials.");
        }

        #region IEnumTypeFieldMembers Members

        public new IEnumeratorType TargetDeclaration
        {
            get
            {
                return (IEnumeratorType)base.TargetDeclaration;
            }
        }

        /// <summary>
        /// Returns the fixed type the field members are defined as.
        /// </summary>
        public ITypeReference EnumType
        {
            get {
                return this.enumType;
            }
        }

        /// <summary>
        /// Inserts a new field by name.
        /// </summary>
        /// <param name="name">The name of the new member.</param>
        /// <returns>A new field member with the <see cref="EnumType"/> and <paramref name="name"/> provided.</returns>
        public IFieldMember AddNew(string name)
        {
            IFieldMember ifm = new FieldMember(new TypedName(name, EnumType), this.TargetDeclaration);
            this.Add(ifm.GetUniqueIdentifier(), ifm);
            ifm.IsConstant = true;
            return ifm;
        }

        /// <summary>
        /// Inserts a new field by name with an initialization expression.
        /// </summary>
        /// <param name="name">The name of the new member.</param>
        /// <param name="initExpression">The expression that initializes the field member.</param>
        /// <returns>A new field member with the <see cref="EnumType"/>, <paramref name="name"/>, and <paramref name="initExpression"/> provided.</returns>
        public IFieldMember AddNew(string name, IExpression initExpression)
        {
            IFieldMember ifm = this.AddNew(name);
            ifm.InitializationExpression = initExpression;
            return ifm;
        }

        #endregion

        #region IFieldMembersBase Members

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, byte initValue)
        {
            switch (this.TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.SByte:
                    if (initValue > SByte.MaxValue)
                        throw new ArgumentOutOfRangeException("Value exceeds enumerator base type.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, sbyte initValue)
        {
            switch (this.TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.UByte:
                case EnumeratorBaseType.UShort:
                case EnumeratorBaseType.UInt:
                case EnumeratorBaseType.ULong:
                    if (initValue < 0)
                        throw new ArgumentOutOfRangeException("initValue", "Unsigned base types cannot contain negative field values.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, uint initValue)
        {
            switch (this.TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    if (initValue > Byte.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned byte.");
                    goto default;
                case EnumeratorBaseType.SByte:
                    if (initValue > SByte.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed byte.");
                    goto default;
                case EnumeratorBaseType.UShort:
                    if (initValue > UInt16.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.Short:
                    if (initValue > Int16.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.SInt:
                    if (initValue > Int32.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 32-bit integer.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, int initValue)
        {
            switch (TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    if (initValue > Byte.MaxValue || initValue < Byte.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned byte.");
                    goto default;
                case EnumeratorBaseType.SByte:
                    if (initValue > SByte.MaxValue || initValue < SByte.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed byte.");
                    goto default;
                case EnumeratorBaseType.UShort:
                    if (initValue > UInt16.MaxValue || initValue < UInt16.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.Short:
                    if (initValue > Int16.MaxValue || initValue < Int16.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 16-bit integer.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, ulong initValue)
        {
            switch (this.TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    if (initValue > Byte.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned byte.");
                    goto default;
                case EnumeratorBaseType.SByte:
                    if (initValue > (ulong)SByte.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed byte.");
                    goto default;
                case EnumeratorBaseType.UShort:
                    if (initValue > UInt16.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.Short:
                    if (initValue > (ulong)Int16.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.UInt:
                    if (initValue > UInt32.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 32-bit integer.");
                    goto default;
                case EnumeratorBaseType.SInt:
                    if (initValue > Int32.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 32-bit integer.");
                    goto default;
                case EnumeratorBaseType.SLong:
                    if (initValue > Int64.MaxValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 64-bit integer.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <see cref="EnumType"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        public IFieldMember AddNew(string name, long initValue)
        {
            switch (this.TargetDeclaration.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    if (initValue > Byte.MaxValue || initValue < Byte.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned byte.");
                    goto default;
                case EnumeratorBaseType.SByte:
                    if (initValue > SByte.MaxValue || initValue < SByte.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed byte.");
                    goto default;
                case EnumeratorBaseType.UShort:
                    if (initValue > UInt16.MaxValue || initValue < UInt16.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.Short:
                    if (initValue > Int16.MaxValue || initValue < Int16.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 16-bit integer.");
                    goto default;
                case EnumeratorBaseType.UInt:
                    if (initValue > UInt32.MaxValue || initValue < UInt32.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of an unsigned 32-bit integer.");
                    goto default;
                case EnumeratorBaseType.SInt:
                    if (initValue > Int32.MaxValue || initValue < Int32.MinValue)
                        throw new ArgumentOutOfRangeException("initValue", "value beyond the range of a signed 32-bit integer.");
                    goto default;
                default:
                    return this.AddNew(name, new PrimitiveExpression(initValue));
            }
        }

        #endregion
    }
}
