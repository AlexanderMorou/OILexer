using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Properties;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    public class IndexerReferenceExpression :
        ParentMemberReference<CodeIndexerExpression>,
        IIndexerReferenceExpression
    {
        private IExpressionCollection indices;
        public IndexerReferenceExpression(IMemberParentExpression reference, IExpressionCollection indices)
            : base(Resources.IndexerName, reference)
        {
            if (indices != null)
                foreach (IExpression exp in indices)
                    this.Indices.Add(exp);
        }

        public IExpressionCollection Indices
        {
            get
            {
                if (this.indices == null)
                    this.indices = new ExpressionCollection();
                return this.indices;
            }
        }

        public override CodeIndexerExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeIndexerExpression(this.Reference.GenerateCodeDom(options), this.Indices.GenerateCodeDom(options));
        }

    }
}
