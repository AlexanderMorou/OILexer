using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// A series of <see cref="IAttributeDeclaration"/> instance implementations.
    /// </summary>
    public class AttributeDeclarations :
        ControlledStateCollection<IAttributeDeclaration>,
        IAttributeDeclarations
    {
        #region IAttributeDeclarations Members

        public bool IsDefined(ITypeReference attributeType)
        {
            if (attributeType.TypeInstance.IsGeneric)
                throw new ArgumentException("Attributes cannot be generic.");
            foreach (IAttributeDeclaration iad in this.baseList)
                if (iad.AttributeType.Equals(iad))
                    return true;
            return false;
        }

        public bool IsDefined(Type attributeType)
        {
            return this.IsDefined(attributeType.GetTypeReference());
        }

        public bool IsDefined(IType attributeType)
        {
            return this.IsDefined(attributeType.GetTypeReference());
        }

        public CodeAttributeDeclarationCollection GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeAttributeDeclaration> result = new List<CodeAttributeDeclaration>();
            foreach (IAttributeDeclaration ia in this)
                result.Add(ia.GenerateCodeDom(options));
            return new CodeAttributeDeclarationCollection (result.ToArray());
        }

        public IAttributeDeclaration AddNew(ITypeReference attributeType)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(IType attributeType)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(Type attributeType)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(ITypeReference attributeType, params IAttributeConstructorParameter[] parameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(IType attributeType, params IAttributeConstructorParameter[] parameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(Type attributeType, params IAttributeConstructorParameter[] parameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(ITypeReference attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters, namedParameters);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(IType attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters, namedParameters);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeDeclaration AddNew(Type attributeType, IAttributeConstructorParameter[] parameters, IAttributePropertyParameter[] namedParameters)
        {
            IAttributeDeclaration item = new AttributeDeclaration(attributeType, parameters, namedParameters);
            this.baseList.Add(item);
            return item;
        }

        #endregion


        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="AttributeDeclarations"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="AttributeDeclarations"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            foreach (IAttributeDeclaration iad in this)
                iad.GatherTypeReferences(ref result, options);
        }

        #endregion

    }
}
