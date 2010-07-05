using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorProductionRuleSeries :
        ProductionRuleSeries,
        IPreprocessorDirective
    {
        public PreprocessorProductionRuleSeries(ICollection<IProductionRule> series, int column, int line, long position)
            : base(series)
        {
            this.Column = column;
            this.Line = line;
            this.Position = position;
        }

        #region IPreprocessorDirective Members

        public int Column { get; set; }

        public int Line { get; set; }

        public long Position { get; set; }

        public EntryPreprocessorType Type { get { return EntryPreprocessorType.ProductionRuleSeries; } }

        #endregion
    }
}
