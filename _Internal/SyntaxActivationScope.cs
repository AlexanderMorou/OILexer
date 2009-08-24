using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Rules;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class SyntaxActivationScope
    {
        private SetCommon.MinimalSetData activeDataPoints;
        private SyntaxActivationInfo info;

        public SyntaxActivationScope(SyntaxActivationInfo info, SimpleLanguageBitArray source)
        {
            this.info = info;
            activeDataPoints = new SetCommon.MinimalSetData(0, (uint)info.Count, info.GetBitArray(source));
            activeDataPoints.Reduce();
        }

        public SyntaxActivationScope(SyntaxActivationScope original)
        {
            this.info = original.info;
            this.activeDataPoints = new SetCommon.MinimalSetData(original.activeDataPoints);
        }

        private SyntaxActivationScope(SyntaxActivationInfo info, SetCommon.MinimalSetData activeDataPoints)
        {
            this.info = info;
            this.activeDataPoints = new SetCommon.MinimalSetData(activeDataPoints);
        }

        public static SyntaxActivationScope operator |(SyntaxActivationScope left, SyntaxActivationScope right)
        {
            return new SyntaxActivationScope(left.info ?? right.info, left.activeDataPoints | right.activeDataPoints);
        }

        public static SyntaxActivationScope operator &(SyntaxActivationScope left, SyntaxActivationScope right)
        {
            return new SyntaxActivationScope(left.info ?? right.info, left.activeDataPoints & right.activeDataPoints);
        }

        public static SyntaxActivationScope operator ^(SyntaxActivationScope left, SyntaxActivationScope right)
        {
            return new SyntaxActivationScope(left.info ?? right.info, left.activeDataPoints ^ right.activeDataPoints);
        }

        public ISyntaxActivationItem[] GetActiveItems()
        {
            return this.activeDataPoints.GetSubSet(this.info.data.DataSet);
        }
    }
}
