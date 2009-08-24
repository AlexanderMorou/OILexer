using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public class AttributePropertyParameter :
        AttributeConstructorParameter,
        IAttributePropertyParameter
    {
        private string name;

        public AttributePropertyParameter(string name, IExpression value)
            : base(value)
        {
            this.name = name;
        }
        
        public override CodeAttributeArgument GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeAttributeArgument result = base.GenerateCodeDom(options);
            result.Name = this.Name;
            return result;
        }

        #region IAttributePropertyParameter Members

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

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            this.name = null;
        }

    }
}
