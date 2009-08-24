using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using System.Linq;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleTokenReferenceItem :
        FlattenedRuleItem
    {
        private SimpleLanguageState state;
        private TokenFinalDataSet tokenLookup;
        private IProductionRuleEntry[] productionRules;
        private TokenFinalData tokenData = null;
        private IRuleDataItem dataItem;
        public new ITokenReferenceProductionRuleItem Source
        {
            get
            {
                return (ITokenReferenceProductionRuleItem)base.Source;
            }
        }

        public FlattenedRuleTokenReferenceItem(ITokenReferenceProductionRuleItem source, IProductionRuleEntry sourceRoot, FlattenedRuleEntry root, FlattenedRuleExpression parent)
            : base(source, sourceRoot, root, parent)
        {
        }

        public override void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            this.productionRules = productionRules;
            this.tokenData = tokenLookup[this.Source.Reference];
            this.tokenLookup = tokenLookup;
        }

        //protected override IRuleDataNode InitializeDataNode()
        //{
        //    if (tokenData.Type == TokenFinalType.Enumeration)
        //        return new RuleDataEnumTokenNode((TokenEnumFinalData)this.tokenData, this.Source);
        //    return new RuleDataCaptureTokenNode(this.Source);
        //}

        public override SimpleLanguageState State
        {
            get
            {
                if (this.state == null)
                {
                    SimpleLanguageState resultState = new SimpleLanguageState();
                    resultState.MoveTo(new SimpleLanguageBitArray(productionRules, tokenLookup.Keys.ToArray(), this.Source.Reference, tokenLookup), new SimpleLanguageState());
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
                    if (this.tokenData is TokenEnumFinalData)
                    {
                        var tokenData = this.tokenData as TokenEnumFinalData;
                        if (string.IsNullOrEmpty(this.Source.Name))
                            this.dataItem = new RuleDataEnumItem(tokenData, this.Source);
                        else
                            this.dataItem = new RuleDataNamedEnumItem(this.Source.Name, tokenData, this.Source);
                    }
                    else
                        if (string.IsNullOrEmpty(this.Source.Name))
                            this.dataItem = new RuleDataCaptureItem(this.Source, this.tokenData);
                        else
                            this.dataItem = new RuleDataNamedCaptureItem(this.Source.Name, this.Source, this.tokenData);
                    if ((this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore) ||
                        (this.RepeatOptions == ScannableEntryItemRepeatOptions.OneOrMore))
                        this.dataItem.Rank++;
                }
                return this.dataItem;
            }
        }
    }
}
