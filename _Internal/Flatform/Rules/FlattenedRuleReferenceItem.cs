using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleReferenceItem :
        FlattenedRuleItem
    {
        private IRuleDataItem dataItem = null;
        private SimpleLanguageState state = null;
        private IProductionRuleEntry[] productionRules;
        private TokenFinalDataSet tokenLookup;
        public new IRuleReferenceProductionRuleItem Source { get { return ((IRuleReferenceProductionRuleItem)(base.Source)); } }

        public FlattenedRuleReferenceItem(IRuleReferenceProductionRuleItem source, IProductionRuleEntry sourceRoot, FlattenedRuleEntry root, FlattenedRuleExpression parent)
            : base(source, sourceRoot, root, parent)
        {
        }

        public override void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            //For state creation.
            this.tokenLookup = tokenLookup;
            this.productionRules = productionRules;
        }

        //protected override IRuleDataNode InitializeDataNode()
        //{
        //    return new RuleDataRuleNode(this.Source);
        ///}

        public override SimpleLanguageState State
        {
            get
            {
                if (this.state == null)
                {
                    SimpleLanguageState resultState = new SimpleLanguageState();
                    var transition = new SimpleLanguageBitArray(productionRules, tokenLookup.Keys.ToArray(), this.Source.Reference, tokenLookup);
                    resultState.MoveTo(transition, new SimpleLanguageState());
                    resultState.OutTransitions.GetNode(transition).Sources.Add(this);
                    this.state = resultState;
                }
                return this.state;
            }
        }

        public override IRuleDataItem DataItem
        {
            get
            {
                if (this.dataItem == null)
                {
                    if (string.IsNullOrEmpty(this.Source.Name))
                        this.dataItem = new RuleDataRuleReferenceItem(this.Source);
                    else
                        this.dataItem = new RuleDataNamedRuleReferenceItem(this.Source.Name, this.Source);
                    if ((this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore) ||
                        (this.RepeatOptions == ScannableEntryItemRepeatOptions.OneOrMore))
                        this.dataItem.Rank++;
                }
                return this.dataItem;
            }
        }
    }
}
