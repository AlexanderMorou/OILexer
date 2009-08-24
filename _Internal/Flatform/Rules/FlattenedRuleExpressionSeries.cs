using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Tokens;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleExpressionSeries :
        Dictionary<IProductionRule, FlattenedRuleExpression>,
        IRuleDataSetSource
    {
        internal IProductionRuleEntry[] productionRules;
        public IProductionRuleSeries Source { get; private set; }
        public IProductionRuleEntry SourceRoot { get; private set; }
        public FlattenedRuleEntry Root { get; private set; }
        public RuleDataSet DataSet { get; private set; }
        //public RuleDataGroupNode DataNode { get; private set; }
        public SimpleLanguageState State { get; internal set; }
        public FlattenedRuleExpressionSeries(IProductionRuleSeries source, IProductionRuleEntry sourceRoot, FlattenedRuleEntry root)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
        }

        public virtual void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            this.Clear();
            this.DataSet = InitializeDataSet();
            //this.DataNode = this.InitializeDataNode();
            this.productionRules = productionRules;
            foreach (var item in this.Source)
                this.Add(item, InitializeElement(item));
            foreach (var item in this.Values)
            {
                item.Initialize(tokenLookup, productionRules);

                if (this.DataSet != null)
                    this.DataSet = this.DataSet.MergeWith(item.DataSet);
                //if (this.DataNode != null)
                //    this.DataNode |= item.DataNode;
            }
        }

        protected virtual RuleDataSet InitializeDataSet()
        {
            return new RuleDataSet(this);
        }

        public virtual void BuildState()
        {
            this.State = this.InitializeState();
            foreach (var item in this.Values)
            {
                item.BuildState();
                if (item.State == null)
                    //No data or state to contribute.
                    continue;
                this.State |= item.State;
            }
        }

        //protected virtual RuleDataGroupNode InitializeDataNode()
        //{
        //    return new RuleDataGroupNode();
        //}

        protected virtual FlattenedRuleExpression InitializeElement(IProductionRule item)
        {
            return new FlattenedRuleExpression(item, SourceRoot, Source, this, Root);
        }

        protected virtual SimpleLanguageState InitializeState()
        {
            return new SimpleLanguageState();
        }
    }
}
