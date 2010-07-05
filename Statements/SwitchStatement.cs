using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;
using Oilexer.Expression;
namespace Oilexer.Statements
{
    public class SwitchStatement :
        Statement<CodeSnippetStatement>,
        ISwitchStatement
    {
        /// <summary>
        /// Data member for <see cref="CaseSwitch"/>.
        /// </summary>
        private IExpression caseSwitch;
        private ISwitchStatementCases cases;

        public SwitchStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {

        }

        public override CodeSnippetStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            throw new NotImplementedException();
        }

        public override void GatherTypeReferences(ref ITypeReferenceCollection result, Oilexer.Translation.ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.cases != null)
                foreach (var @case in this.cases)
                    @case.GatherTypeReferences(ref result, options);
        }

        #region ISwitchStatement Members

        public IExpression CaseSwitch
        {
            get
            {
                return this.caseSwitch;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("caseSwitch");
                this.caseSwitch = value;
            }
        }

        public ISwitchStatementCases Cases
        {
            get
            {
                if (this.cases == null)
                    this.cases = new SwitchStatementCases(this.SourceBlock);
                return this.cases;
            }
        }

        #endregion

    }
}
