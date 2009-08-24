using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Rules;
using System.Linq;
using System.Diagnostics;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    internal sealed partial class SimpleLanguageRuleState :
        SimpleLanguageState
    {
        public FlattenedRuleEntry Source { get; private set; }

        public SimpleLanguageRuleState(FlattenedRuleEntry source)
        {
            this.Source = source;
        }

        internal override SimpleLanguageState GetNewInstance()
        {
            return new SimpleLanguageRuleState(this.Source);
        }

        internal override void MakeEdge()
        {
            base.MakeEdge();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Source.GetHashCode();
        }

    }
}
