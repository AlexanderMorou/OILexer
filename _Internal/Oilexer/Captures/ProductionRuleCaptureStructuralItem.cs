using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal abstract class ProductionRuleCaptureStructuralItem :
        IProductionRuleCaptureStructuralItem
    {
        private string name;
        private readonly IOilexerGrammarProductionRuleEntry rule;

        public ProductionRuleCaptureStructuralItem(IOilexerGrammarProductionRuleEntry rule)
        {
            this.rule = rule;
        }

        #region IProductionRuleCaptureStructuralItem Members

        public IControlledCollection<IProductionRuleSource> Sources { get { return this.OnGetSources(); } }

        protected abstract IControlledCollection<IProductionRuleSource> OnGetSources();

        public virtual ResultedDataType ResultType { get; set; }

        public IProductionRuleCaptureStructuralItem Union(IProductionRuleCaptureStructuralItem rightElement)
        {
            return this.PerformUnion(rightElement);
        }

        protected abstract IProductionRuleCaptureStructuralItem PerformUnion(IProductionRuleCaptureStructuralItem rightElement);

        public string Name
        {
            get
            {
                return this.name ?? (this.name = (from s in this.Sources
                                                  let iti = s as IProductionRuleItem
                                                  where iti != null &&
                                                        !string.IsNullOrEmpty(iti.Name)
                                                  select iti.Name).FirstOrDefault());
            }
        }

        public string BucketName
        {
            get
            {
                return this.BucketIndex == null ? this.Name : string.Format("{0}{1}", this.Name, this.BucketIndex);
            }
        }

        public int? BucketIndex { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.ResultType, this.BucketName);
        }

        #endregion

        internal ResultedDataType UnionResultTypes(IProductionRuleCaptureStructuralItem rightElement)
        {
            switch (this.ResultType)
            {
                case ResultedDataType.None:
                    return rightElement.ResultType;
                case ResultedDataType.EnumerationItem:
                    if (rightElement.ResultType == ResultedDataType.FlagEnumerationItem)
                        return rightElement.ResultType;
                    break;
                case ResultedDataType.ImportType:
                    if (rightElement.ResultType == ResultedDataType.ImportTypeList)
                        return ResultedDataType.ImportTypeList;
                    break;
            }
            return this.ResultType;
        }
        public IIntermediateFieldMember AssociatedField { get; set; }

        public int StateIndex { get; set; }

        public bool Optional { get; set; }

        public bool GroupOptional { get; set; }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.rule; }
        }
    }
}
