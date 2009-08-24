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
    public class CastExpression :
        MemberParentExpression<CodeCastExpression>,
        ICastExpression,
        ISerializable
    {
        IExpression target;
        ITypeReference type;
        public CastExpression(IExpression target, ITypeReference type)
        {
            this.target = target;
            this.type = type;
        }

        public CastExpression(SerializationInfo info, StreamingContext context)
        {
            this.target = _SerializationMethods.DeserializeExpression("Target", info);
            this.type = _SerializationMethods.DeserializeTypeReference("Type", info);
        }

        #region ICastExpression Members

        public IExpression Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        public ITypeReference Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        #endregion

        #region Expression Members

        public override CodeCastExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeCastExpression result = base.GenerateCodeDom(options);
            result.Expression = this.Target.GenerateCodeDom(options);
            result.TargetType = this.Type.GenerateCodeDom(options);
            return result;
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="CastExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="CastExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (this.target != null)
                this.target.GatherTypeReferences(ref result, options);
            if (this.type != null)
                result.Add(this.type);
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _SerializationMethods.SerializeExpression("Target", info, this.target);
            _SerializationMethods.SerializeTypeReference("Type", info, this.type);
        }

        #endregion
    }
}
