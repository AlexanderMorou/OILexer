using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SoftTemplateReferenceProductionRuleItem :
        SoftReferenceProductionRuleItem,
        ISoftTemplateReferenceProductionRuleItem
    {
        
        private IControlledCollection<IProductionRuleSeries> parts;
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

            this.parts = new AllenCopeland.Abstraction.Utilities.Collections.ReadOnlyCollection<IProductionRuleSeries>(parts);
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

        public IControlledCollection<IProductionRuleSeries> Parts
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
            AllenCopeland.Abstraction.Utilities.Collections.ReadOnlyCollection<IProductionRule>,
            IProductionRuleSeries
        {
            public SeriesEntry(IList<IProductionRule> baseCollection)
                : base(baseCollection)
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
