using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Statements;
using Oilexer._Internal;
using Microsoft.CSharp;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// Provides a base implementation for the <see cref="IExpression"/> interface.
    /// </summary>
    [Serializable]
    /* *
     * CS0660 and CS0661 are intentional and by design.
     * */
    public abstract class Expression :
        IExpression
    {

        #region IExpression Members

        public CodeExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.OnGenerateCodeDom(options);
        }

        protected abstract CodeExpression OnGenerateCodeDom(ICodeDOMTranslationOptions options);

        #endregion
        #region Expression + Expression Operators
        public static BinaryOperationExpression operator ==(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.ValueEquality, exp2);
        }

        public static BinaryOperationExpression operator !=(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.IdentityInequality, exp2);
        }

        public static BinaryOperationExpression operator *(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Multiply, exp2);
        }

        public static BinaryOperationExpression operator /(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Divide, exp2);
        }

        public static BinaryOperationExpression operator +(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Add, exp2);
        }

        public static BinaryOperationExpression operator -(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Subtract, exp2);
        }


        public static BinaryOperationExpression operator |(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.BitwiseOr, exp2);
        }

        public static BinaryOperationExpression operator &(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.BitwiseAnd, exp2);
        }

        public static BinaryOperationExpression operator <(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.LessThan, exp2);
        }


        public static BinaryOperationExpression operator <=(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.LessThanOrEqual, exp2);
        }

        public static BinaryOperationExpression operator >(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.GreaterThan, exp2);
        }

        public static BinaryOperationExpression operator >=(Expression exp1, Expression exp2)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.GreaterThanOrEqual, exp2);
        }
        #endregion

        #region Member + Expression operators

        public static BinaryOperationExpression operator ==(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.ValueEquality, exp2);
        }

        public static BinaryOperationExpression operator !=(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.IdentityInequality, exp2);
        }


        public static BinaryOperationExpression operator *(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.Multiply, exp2);
        }

        public static BinaryOperationExpression operator /(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.Divide, exp2);
        }

        public static BinaryOperationExpression operator +(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.Add, exp2);
        }

        public static BinaryOperationExpression operator -(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.Subtract, exp2);
        }


        public static BinaryOperationExpression operator <(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.LessThan, exp2);
        }


        public static BinaryOperationExpression operator <=(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.LessThanOrEqual, exp2);
        }

        public static BinaryOperationExpression operator >(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.GreaterThan, exp2);
        }

        public static BinaryOperationExpression operator >=(IMember localMember, Expression exp2)
        {
            return new BinaryOperationExpression(localMember.GetReference(), CodeBinaryOperatorType.GreaterThanOrEqual, exp2);
        }
        #endregion

        #region Expression + Member Operators
        public static BinaryOperationExpression operator ==(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.ValueEquality, localMember.GetReference());
        }

        public static BinaryOperationExpression operator !=(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.IdentityInequality, localMember.GetReference());
        }


        public static BinaryOperationExpression operator *(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Multiply, localMember.GetReference());
        }

        public static BinaryOperationExpression operator /(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Divide, localMember.GetReference());
        }

        public static BinaryOperationExpression operator +(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Add, localMember.GetReference());
        }

        public static BinaryOperationExpression operator -(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.Subtract, localMember.GetReference());
        }


        public static BinaryOperationExpression operator |(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.BitwiseOr, localMember.GetReference());
        }

        public static BinaryOperationExpression operator &(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.BitwiseAnd, localMember.GetReference());
        }

        public static BinaryOperationExpression operator <(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.LessThan, localMember.GetReference());
        }


        public static BinaryOperationExpression operator <=(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.LessThanOrEqual, localMember.GetReference());
        }

        public static BinaryOperationExpression operator >(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.GreaterThan, localMember.GetReference());
        }

        public static BinaryOperationExpression operator >=(Expression exp1, IMember localMember)
        {
            return new BinaryOperationExpression(exp1, CodeBinaryOperatorType.GreaterThanOrEqual, localMember.GetReference());
        }


        #endregion

        public static BinaryOperationExpression operator -(Expression exp)
        {
            return new BinaryOperationExpression(null, CodeBinaryOperatorType.Subtract, exp);
        }

        public static implicit operator Expression(string exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(bool exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(byte exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(sbyte exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(int exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(uint exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(char exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(short exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(ushort exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(long exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(ulong exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(float exp)
        {
            return new PrimitiveExpression(exp);
        }
        public static implicit operator Expression(double exp)
        {
            return new PrimitiveExpression(exp);
        }

        public override string ToString()
        {
            return _OIL._Core.GenerateExpressionSnippet(CodeGeneratorHelper.DefaultDomOptions, this.GenerateCodeDom(CodeGeneratorHelper.DefaultDomOptions));
        }

        #region IExpression Members

        /// <summary>
        /// Performs a look-up on the <see cref="Expression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="Expression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public abstract void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options);

        #endregion
    }
}
