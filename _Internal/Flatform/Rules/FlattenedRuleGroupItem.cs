using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleGroupItem :
        FlattenedRuleExpressionSeries,
        IFlattenedRuleItem
    {
        public new IProductionRuleGroupItem Source
        {
            get
            {
                return ((IProductionRuleGroupItem)(base.Source));
            }
        }

        public FlattenedRuleExpression Parent { get; private set; }

        public FlattenedRuleGroupItem(IProductionRuleGroupItem source, IProductionRuleEntry sourceRoot, FlattenedRuleExpression parent, FlattenedRuleEntry root)
            : base(source, sourceRoot, root)
        {
            this.Parent = parent;
        }

        #region IFlattenedRuleItem Members

        IProductionRuleItem IFlattenedRuleItem.Source
        {
            get { return this.Source; }
        }

        #endregion

        #region IFlattenedRuleItem Members

        //public IRuleDataNode DataNode
        //{
        //    get
        //    {
        //        var result = base.DataNode;
        //        return base.DataNode;
        //    }
        //}

        #endregion

        //protected override RuleDataGroupNode InitializeDataNode()
        //{
        //    return new RuleDataGroupNode(this.Source);
        //}


        #region IFlattenedRuleItem Members


        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get { return this.Source.RepeatOptions; }
        }

        #endregion

        #region IFlattenedRuleItem Members


        public bool Optional
        {
            get
            {
                /* *
                 * If only one expression is optional, in some cases the group
                 * can be considered optional.
                 * */
                bool alreadyOptional = this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                                       this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
                if (alreadyOptional)
                    return true;
                foreach (var expression in this.Values)
                {
                    bool overallOptional = true;
                    foreach (var item in expression.Values)
                    {
                        overallOptional = item.Optional;
                        if (!overallOptional)
                            break;
                    }
                    if (overallOptional)
                        return true;
                }
                return false;
                
                ///* *
                // * If every expression of the source contains nothing
                // * but optional items, then the group, regardless of its own repeat
                // * options, is optional.
                // * */
                //foreach (var expression in this.Keys)
                //    foreach (var item in expression)
                //        if (item.RepeatOptions != ScannableEntryItemRepeatOptions.ZeroOrMore &&
                //            item.RepeatOptions != ScannableEntryItemRepeatOptions.ZeroOrOne)
                //            return this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                //                   this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
                //return true;
            }
        }

        #endregion

        #region IFlattenedRuleItem Members

        public IRuleDataItem DataItem
        {
            get { return null; }
        }

        #endregion

        public override void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            base.Initialize(tokenLookup, productionRules);
            if ((this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore) ||
                (this.RepeatOptions == ScannableEntryItemRepeatOptions.OneOrMore))
                base.DataSet.AddRank();
        }

    }
}
