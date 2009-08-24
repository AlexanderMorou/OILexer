using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using System.Runtime.Serialization;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class BinaryOperationExpression :
        Expression<CodeBinaryOperatorExpression>,
        IBinaryOperationExpression,
        ISerializable
    {
        /// <summary>
        /// Data member for <see cref="LeftSide"/>.
        /// </summary>
        private IExpression leftSide; 
        /// <summary>
        /// Data member for <see cref="Operation"/>.
        /// </summary>
        private CodeBinaryOperatorType operation; 
        /// <summary>
        /// Data member for <see cref="RightSide"/>.
        /// </summary>
        private IExpression rightSide;
        /// <summary>
        /// Creates a new instance of <see cref="BinaryOperationExpression"/> which performs a binary
        /// operation.
        /// </summary>
        /// <param name="leftSide">The left operand.</param>
        /// <param name="operation">The operation to perform.</param>
        /// <param name="rightSide">The right operand.</param>
        public BinaryOperationExpression (IExpression leftSide, CodeBinaryOperatorType operation, IExpression rightSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            this.operation = operation;
        }

        public BinaryOperationExpression(SerializationInfo info, StreamingContext context)
        {
            this.leftSide = _SerializationMethods.DeserializeExpression("LeftSide", info);
            this.operation = (CodeBinaryOperatorType)info.GetValue("Operation", typeof(CodeBinaryOperatorType));
            this.rightSide = _SerializationMethods.DeserializeExpression("RightSide", info);
        }

        public override CodeBinaryOperatorExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (this.Operation == CodeBinaryOperatorType.Subtract)
                if (this.LeftSide == null)
                    return new CodeBinaryOperatorExpression(new CodeSnippetExpression(), CodeBinaryOperatorType.Subtract, this.RightSide.GenerateCodeDom(options));
                else
                    return new CodeBinaryOperatorExpression(this.LeftSide.GenerateCodeDom(options), CodeBinaryOperatorType.Subtract, this.RightSide.GenerateCodeDom(options));
            else
                if (this.LeftSide == null)
                    throw new InvalidOperationException("Left operand null...");
                else if (this.RightSide == null)
                    throw new InvalidOperationException("Right operand null...");
                else
                    return new CodeBinaryOperatorExpression(this.LeftSide.GenerateCodeDom(options), this.Operation, this.RightSide.GenerateCodeDom(options));
        }

        #region IBinaryOperationExpression Members

        public IExpression LeftSide
        {
            get
            {
                return this.leftSide;
            }
            set
            {
                this.leftSide = value;
            }
        }

        public CodeBinaryOperatorType Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }

        public IExpression RightSide
        {
            get
            {
                return this.rightSide;
            }
            set
            {
                this.rightSide = value;
            }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="BinaryOperationExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="BinaryOperationExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.leftSide != null)
                this.leftSide.GatherTypeReferences(ref result, options);
            if (this.rightSide != null)
                this.RightSide.GatherTypeReferences(ref result, options);
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _SerializationMethods.SerializeExpression("LeftSide", info, this.leftSide);
            info.AddValue("Operation", this.operation, typeof(CodeBinaryOperatorType));
            _SerializationMethods.SerializeExpression("RightSide", info, this.rightSide);
        }

        #endregion
    }
}
