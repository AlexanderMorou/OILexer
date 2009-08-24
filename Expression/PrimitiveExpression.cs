using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class PrimitiveExpression :
        MemberParentExpression<CodePrimitiveExpression>,
        IPrimitiveExpression
    {
        /// <summary>
        /// Data member for <see cref="Value"/>.
        /// </summary>
        private object value;
        /// <summary>
        /// Creates a new instance of <see cref="PrimitiveExpression"/>
        /// </summary>
        public PrimitiveExpression()
        {
            this.value = null;
        }
        /// <summary>
        /// Represents a common primitive null expression of &lt;null&gt;.
        /// </summary>
        public static readonly PrimitiveExpression NullValue = new PrimitiveExpression(null);
        /// <summary>
        /// Represents a common primitive <see cref="System.String"/> expression of <see cref="String.Empty"/>.
        /// </summary>
        public static readonly PrimitiveExpression EmptyString = new PrimitiveExpression(string.Empty);
        /// <summary>
        /// Represents a common primitive <see cref="System.Int32"/> expression of '0'.
        /// </summary>
        public static readonly PrimitiveExpression NumberZero = new PrimitiveExpression(0);
        /// <summary>
        /// Represents a common primitive boolean expression of 'true'.
        /// </summary>
        public static readonly PrimitiveExpression TrueValue = new PrimitiveExpression(true);
        /// <summary>
        /// Represents a common primitive boolean expression of 'false'.
        /// </summary>
        public static readonly PrimitiveExpression FalseValue = new PrimitiveExpression(false);

        /// <summary>
        /// Creates a new instance of <see cref="PrimitiveExpression"/> with the 
        /// <paramref name="value"/> provided.
        /// </summary>
        /// <param name="value">The value of the <see cref="PrimitiveExpression"/>.</param>
        public PrimitiveExpression(object value)
        {
            this.value = value;
        }

        #region IPrimitiveExpression Members
        /// <summary>
        /// Returns/sets the value of the <see cref="PrimitiveExpression"/>.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public TypeCode TypeCode
        {
            get {
                if (this.value == null)
                    return TypeCode.Empty;
                else if (this.value is string)
                    return TypeCode.String;
                else if (this.value is int)
                    return TypeCode.Int32;
                else if (this.value is uint)
                    return TypeCode.UInt32;
                else if (this.value is ushort)
                    return TypeCode.UInt16;
                else if (this.value is short)
                    return TypeCode.Int16;
                else if (this.value is long)
                    return TypeCode.Int64;
                else if (this.value is ulong)
                    return TypeCode.UInt64;
                else if (this.value is double)
                    return TypeCode.Double;
                else if (this.value is float)
                    return TypeCode.Single;
                else if (this.value is byte)
                    return TypeCode.Byte;
                else if (this.value is sbyte)
                    return TypeCode.SByte;
                else if (this.value is bool)
                    return TypeCode.Boolean;
                else if (this.value is char)
                    return TypeCode.Char;
                else
                    return TypeCode.Object;
            }
        }

        #endregion

        public override CodePrimitiveExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodePrimitiveExpression(this.value);
        }

        /// <summary>
        /// Performs a look-up on the <see cref="PrimitiveExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="PrimitiveExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        /// <remarks>Primitives carry no impact beyond native types.</remarks>
        public override void GatherTypeReferences(ref Oilexer.Types.ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            //Primitives carry no impact beyond native types.
            return;
        }
    }
}
