using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal abstract class RuleDataTokenItemBase :
        RuleDataItem,
        IRuleDataTokenItem
    {
        private TokenFinalData finalData;
        public RuleDataTokenItemBase(ITokenEntry source, TokenFinalData finalData)
            : base()
        {
            this.finalData = finalData;
            this.Source = source;
        }

        #region IRuleDataTokenItem Members

        public ITokenEntry Source { get; internal set; }

        #endregion
        public override string ToString()
        {
            if (this.finalData.TokenInterface != null)
            {
                return string.Format("{0}{1}", this.finalData.TokenInterface.Name, this.Rank > 0 ? GetRankText() : string.Empty);
            }
            return null;
        }

        protected TokenFinalData FinalData
        {
            get
            {
                return this.finalData;
            }
        }
    }
}
