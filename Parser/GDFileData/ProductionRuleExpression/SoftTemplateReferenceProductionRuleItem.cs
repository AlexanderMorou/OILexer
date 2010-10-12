using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.Linq;
using System.Collections.ObjectModel;
namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public class SoftTemplateReferenceProductionRuleItem :
        SoftReferenceProductionRuleItem,
        ISoftTemplateReferenceProductionRuleItem
    {
        
        private IReadOnlyCollection<IProductionRuleSeries> parts;
        /// <summary>
        /// Creates a new <see cref="SoftTemplateReferenceProductionRuleItem"/> instance
        /// with the <paramref name="serii"/>, <paramref name="primaryName"/>, 
        /// <paramref name="line"/>, <paramref name="column"/>, and <paramref name="position"/>
        /// provided.
        /// </summary>
        /// <param name="primaryName">The token, declaration rule, or error that the <see cref="SoftTemplateReferenceProductionRuleItem"/>
        /// refers to.</param>
        /// <param name="line">The line at which the <see cref="SoftTemplateReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="SoftTemplateReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="SoftTemplateReferenceProductionRuleItem"/> was declared.</param>
        /// <remarks><paramref name="secondaryName"/> does not relate to errors because
        /// errors have no members.</remarks>
        public SoftTemplateReferenceProductionRuleItem(IList<IProductionRule>[] serii, string primaryName, int line, int column, long position)
            : base(primaryName, null, line, column, position, false, false)
        {
            IList<IProductionRuleSeries> parts = new List<IProductionRuleSeries>();
            foreach (IList<IProductionRule> c in serii)
                parts.Add(new SeriesEntry(c));

            this.parts = new Oilexer.Utilities.Collections.ReadOnlyCollection<IProductionRuleSeries>(parts);
        }

        protected override object OnClone()
        {
            SoftTemplateReferenceProductionRuleItem srpri = new SoftTemplateReferenceProductionRuleItem((from series in this.parts
                                                                                                         select new Collection<IProductionRule>(series.ToArray())).ToArray(), this.PrimaryName, this.Line, this.Column, this.Position)
            {
                PrimaryToken = this.PrimaryToken,
                SecondaryToken = this.SecondaryToken
            };

            base.CloneData(srpri);
            return srpri;
        }

        #region ISoftTemplateReferenceProductionRuleItem Members

        public IReadOnlyCollection<IProductionRuleSeries> Parts
        {
            get { return this.parts; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<"); 
            bool first = true;
            foreach (IProductionRuleSeries s in this.Parts)
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(s.ToString());
            }
            sb.Append(">");
            return string.Format("{0}{1}", base.ToString(),sb.ToString());
        }

        #endregion
        private class SeriesEntry :
            Oilexer.Utilities.Collections.ReadOnlyCollection<IProductionRule>,
            IProductionRuleSeries
        {
            public SeriesEntry(IList<IProductionRule> baseList)
                : base(baseList)
            {

            }
            public override string ToString()
            {
                bool first = true;
                StringBuilder sb = new StringBuilder();
                foreach (IProductionRule ipr in this.baseList)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(" | ");
                    sb.Append(ipr.ToString());
                }
                return sb.ToString();
            }

            #region IProductionRuleSeries Members

            public string GetBodyString()
            {
                return this.ToString();
            }

            #endregion

        }
    }
}
