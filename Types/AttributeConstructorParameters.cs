using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;
using Oilexer.Utilities.Arrays;
using System.CodeDom;
using Oilexer.Translation;
using System.Linq;

namespace Oilexer.Types
{
    public class AttributeConstructorParameters :
        ControlledStateCollection<IAttributeConstructorParameter>,
        IAttributeConstructorParameters
    {
        #region IAttributeConstructorParameters Members

        public CodeAttributeArgumentCollection GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            IAttributeConstructorParameter[] normal = (from a in this
                                                       where (!(a is IAttributePropertyParameter))
                                                       select a).ToArray();
            IAttributeConstructorParameter[] named = (from a in this
                                                      where (a is IAttributePropertyParameter)
                                                      select a).ToArray();
            List<CodeAttributeArgument> result = new List<CodeAttributeArgument>();
            foreach (IAttributeConstructorParameter param in normal)
                result.Add(param.GenerateCodeDom(options));
            foreach (IAttributeConstructorParameter param in named)
                result.Add(param.GenerateCodeDom(options));
            return new CodeAttributeArgumentCollection(result.ToArray());
        }

        public IAttributePropertyParameter AddNew(string name, IExpression value)
        {
            IAttributePropertyParameter item = new AttributePropertyParameter(name, value);
            this.baseList.Add(item);
            return item;
        }

        public IAttributeConstructorParameter AddNew(IExpression value)
        {
            IAttributeConstructorParameter item = new AttributeConstructorParameter(value);
            this.baseList.Add(item);
            return item;
        }

        public void AddRange(params IAttributeConstructorParameter[] parameters)
        {
            foreach (IAttributeConstructorParameter param in parameters)
                this.baseList.Add(param);
        }

        public void AddRange(params IAttributePropertyParameter[] parameters)
        {
            foreach (IAttributeConstructorParameter param in parameters)
                this.baseList.Add(param);
        }

        #endregion

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="AttributeConstructorParameters"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="AttributeConstructorParameters"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            foreach (IAttributeConstructorParameter iacp in this)
                iacp.GatherTypeReferences(ref result, options);
        }

        #endregion

        #region IAttributeConstructorParameters Members


        public void Clear()
        {
            this.baseList.Clear();
        }

        public void Remove(IAttributeConstructorParameter parameter)
        {
            this.baseList.Remove(parameter);
        }

        public void Remove(IAttributePropertyParameter parameter)
        {
            this.baseList.Remove(parameter);
        }

        #endregion

    }
}
