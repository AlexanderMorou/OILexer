using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class CreateNewObjectExpression :
        MemberParentExpression<CodeObjectCreateExpression>,
        ICreateNewObjectExpression,
        ISerializable
    {
        /// <summary>
        /// Data member for <see cref="NewType"/>
        /// </summary>
        private ITypeReference newType;
        /// <summary>
        /// Data member for <see cref="Arguments"/>.
        /// </summary>
        private IExpressionCollection arguments;

        public CreateNewObjectExpression(ITypeReference newType, IExpressionCollection arguments)
            : this(newType, (IEnumerable<IExpression>)(arguments))
        {
        }

        public CreateNewObjectExpression(ITypeReference newType, params IExpression[] arguments)
            : this(newType, (IEnumerable<IExpression>)(arguments))
        {
        }

        public CreateNewObjectExpression(ITypeReference newType, IEnumerable<IExpression> arguments)
        {
            this.newType = newType;
            this.arguments = new ExpressionCollection(arguments);
        }

        protected CreateNewObjectExpression(SerializationInfo info, StreamingContext context)
        {
            this.newType = _SerializationMethods.DeserializeTypeReference("NewType", info);
            this.arguments = _SerializationMethods.DeserializeExpressionCollection("Arguments", info);
        }

        public override CodeObjectCreateExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeObjectCreateExpression(NewType.GenerateCodeDom(options), Arguments.GenerateCodeDom(options));
        }

        #region ICreateNewObjectExpression Members

        public ITypeReference NewType
        {
            get
            {
                return this.newType;
            }
            set
            {
                this.newType = value;
            }
        }

        public IExpressionCollection Arguments
        {
            get
            {
                if (this.arguments == null)
                    this.arguments = new ExpressionCollection();
                return this.arguments;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="CreateNewObjectExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="CreateNewObjectExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.newType != null)
                result.Add(this.NewType);
            if (this.arguments != null)
                this.arguments.GatherTypeReferences(ref result, options);
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _SerializationMethods.SerializeTypeReference("NewType", info, this.newType);
            _SerializationMethods.SerializeExpressionCollection("Arguments", info, this.arguments);
        }

        #endregion
    }
}
