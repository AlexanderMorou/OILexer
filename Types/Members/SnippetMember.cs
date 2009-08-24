using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class SnippetMember :
        Member<IMemberParentType, CodeSnippetTypeMember>,
        ISnippetMember
    {
        private string text;
        public SnippetMember(IMemberParentType parent)
            : base(null, parent)
        {
        }
        protected override IMemberReferenceExpression OnGetReference()
        {
            return null;
        }

        public override CodeSnippetTypeMember GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeSnippetTypeMember(this.Text);
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Performs a look-up on the <see cref="SnippetMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="SnippetMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            base.GatherTypeReferences(ref result, options);
            //Since snippet members are text, they carry no default type dependency.
            return;
        }
    }
}
