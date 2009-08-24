﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using System.IO;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleCharLiteralReferenceItem :
        FlattenedRuleItem
    {
        private SimpleLanguageState state;
        private TokenEnumFinalData finalData;
        private ProjectConstructor.EnumStateMachineData elementData;
        private IProductionRuleEntry[] productionRules;
        private IRuleDataItem dataItem;
        private TokenFinalDataSet tokenLookup;
        public FlattenedRuleCharLiteralReferenceItem(ILiteralCharReferenceProductionRuleItem source, IProductionRuleEntry sourceRoot, FlattenedRuleEntry root, FlattenedRuleExpression parent)
            : base(source, sourceRoot, root, parent)
        {
        }

        /// <summary>
        /// Returns the <see cref="ILiteralCharReferenceProductionRuleItem"/>
        /// from which the current <see cref="FlattenedRuleCharLiteralReferenceItem"/> is derived.
        /// </summary>
        public new ILiteralCharReferenceProductionRuleItem Source
        {
            get
            {
                return ((ILiteralCharReferenceProductionRuleItem)(base.Source));
            }
        }

        public override void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            this.tokenLookup = tokenLookup;
            var tokenData = tokenLookup[Source.Source];
            this.productionRules = productionRules;
            this.finalData = tokenData as TokenEnumFinalData;
            if (this.finalData == null)
                return;
            this.elementData = this.finalData.FinalItemLookup[this.Source.Literal];
        }

        //protected override IRuleDataNode InitializeDataNode()
        //{
        //    return new RuleDataEnumTokenNode(this.finalData, this.Source);
        //}

        public override SimpleLanguageState State
        {
            get
            {
                if (this.state == null)
                {
                    SimpleLanguageBitArray transition = new SimpleLanguageBitArray(this.productionRules, this.tokenLookup.Keys.ToArray(), this.Source.Literal, tokenLookup);
                    SimpleLanguageState state = new SimpleLanguageState();
                    state.MoveTo(transition, new SimpleLanguageState());
                    state.OutTransitions.GetNode(transition).Sources.Add(this);
                    this.state = state;
                }
                return this.state;
            }
        }

        public override IRuleDataItem DataItem
        {
            get
            {
                if (dataItem == null)
                {
                    if (string.IsNullOrEmpty(this.Source.Name))
                        this.dataItem = new RuleDataEnumItem(this.finalData, this.Source);
                    else
                        this.dataItem = new RuleDataNamedEnumItem(this.Source.Name, this.finalData, this.Source);
                    if ((this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore) ||
                        (this.RepeatOptions == ScannableEntryItemRepeatOptions.OneOrMore))
                        this.dataItem.Rank++;
                }
                return this.dataItem;
            }
        }
    }
}
