using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public class AttributeConstructorParameter :
        IAttributeConstructorParameter,
        ISerializable
    {
        /// <summary>
        /// Data member for <see cref="Value"/>.
        /// </summary>
        private IExpression value;

        public AttributeConstructorParameter(IExpression value)
        {
            this.value = value;
        }

        protected AttributeConstructorParameter(SerializationInfo info, StreamingContext context)
        {
            this.value = _SerializationMethods.DeserializeExpression("Value", info);
        }


        #region IAttributeConstructorParameter Members

        public virtual CodeAttributeArgument GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeAttributeArgument(this.value.GenerateCodeDom(options));
        }

        /// <summary>
        /// Returns/sets the value of the <see cref="AttributeConstructorParameter"/>.
        /// </summary>
        public IExpression Value
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

        #endregion

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="AttributeConstructorParameter"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="AttributeConstructorParameter"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.value != null)
                this.Value.GatherTypeReferences(ref result, options);
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _SerializationMethods.SerializeExpression("Value", info, this.value);
        }

        #endregion


        #region IDisposable Members

        public virtual void Dispose()
        {
            this.value = null;
        }

        #endregion
    }
}
