using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal abstract class FlattenedRuleItem :
        IFlattenedRuleItem
    {
        //private IRuleDataNode dataNode = null;
        public FlattenedRuleItem(IProductionRuleItem source, IProductionRuleEntry sourceRoot, FlattenedRuleEntry root, FlattenedRuleExpression parent)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.Parent = parent;
        }

        #region IFlattenedRuleItem Members

        public IProductionRuleItem Source { get; private set; }

        public IProductionRuleEntry SourceRoot { get; private set; }

        public FlattenedRuleEntry Root { get; private set; }

        public FlattenedRuleExpression Parent { get; private set; }

        public abstract void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules);

        //public IRuleDataNode DataNode
        //{
        //    get {
        //        if (this.dataNode == null)
        //            this.dataNode = this.InitializeDataNode();
        //        return this.dataNode;
        //    }
        //}

        //protected abstract IRuleDataNode InitializeDataNode();

        public abstract SimpleLanguageState State { get; }

        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get { return this.Source.RepeatOptions; }
        }
        public bool Optional
        {
            get
            {
                return this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                       this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
            }
        }

        public abstract IRuleDataItem DataItem { get; }

        #endregion
    }
}
