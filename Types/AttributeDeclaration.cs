using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public class AttributeDeclaration :
        IAttributeDeclaration
    {
        private string name;
        private ITypeReference attributeType;
        private IAttributeConstructorParameters parameters;
        #region AttributeDeclaration Constructors
        public AttributeDeclaration(ITypeReference attributeType):this(attributeType, new IAttributeConstructorParameter[0]) { }
        public AttributeDeclaration(IType attributeType) : this(attributeType, new IAttributeConstructorParameter[0]) { }
        public AttributeDeclaration(Type attributeType) : this(attributeType, new IAttributeConstructorParameter[0]) { }
        public AttributeDeclaration(ITypeReference attributeType, params IAttributeConstructorParameter[] parameters) : this(attributeType, parameters, new IAttributePropertyParameter[0]) { }
        public AttributeDeclaration(IType attributeType, params IAttributeConstructorParameter[] parameters) : this(attributeType, parameters, new IAttributePropertyParameter[0]) { }
        public AttributeDeclaration(Type attributeType, params IAttributeConstructorParameter[] parameters) : this(attributeType, parameters, new IAttributePropertyParameter[0]) { }
        public AttributeDeclaration(IType attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters):this(attributeType.GetTypeReference(), parameters, namedParameters) { }
        public AttributeDeclaration(Type attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters) : this(attributeType.GetTypeReference(), parameters, namedParameters) { }
        public AttributeDeclaration(ITypeReference attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters) 
        {
            this.attributeType = attributeType;
            this.Parameters.AddRange(parameters);
            this.Parameters.AddRange(namedParameters);
        }


        #endregion

        #region IAttributeDeclaration Members

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public ITypeReference AttributeType
        {
            get
            {
                return this.attributeType;
            }
            set
            {
                this.attributeType = value;
            }
        }

        public IAttributeConstructorParameters Parameters
        {
            get {
                if (this.parameters == null)
                    this.InitializeParameters();
                return this.parameters;
            }
        }

        private void InitializeParameters()
        {
            this.parameters = new AttributeConstructorParameters();
        }

        public CodeAttributeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeAttributeDeclaration result = new CodeAttributeDeclaration(this.AttributeType.GenerateCodeDom(options));
            result.Arguments.AddRange(this.Parameters.GenerateCodeDom(options));
            return result;
        }

        #endregion


        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="AttributeDeclaration"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="AttributeDeclaration"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.attributeType != null)
                result.Add(this.AttributeType);
            if (this.parameters != null)
                this.parameters.GatherTypeReferences(ref result, options);
        }

        #endregion
    }
}
