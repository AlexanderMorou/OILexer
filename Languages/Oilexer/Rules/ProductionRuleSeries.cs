using System;
using System.Collections.Generic;
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
    public class ProductionRuleSeries :
        ReadOnlyCollection<IProductionRule>,
        IProductionRuleSeries
    {
        /// <summary>
        /// Creates a new <see cref="ProductionRuleSeries"/> instance with the
        /// <paramref name="series"/> to include provided.
        /// </summary>
        /// <param name="series">The series of <see cref="IProductionRule"/>
        /// expressions to contain.</param>
        public ProductionRuleSeries(ICollection<IProductionRule> series)
        {
            foreach (var expression in series)
                this.baseList.Add(expression);
        }

        /// <summary>
        /// Creates a new <see cref="ProductionRuleSeries"/> instance with
        /// the <paramref name="serii"/> to include provided.
        /// </summary>
        /// <param name="serii">The set of expression series to include.</param>
        public ProductionRuleSeries(ICollection<IProductionRuleSeries> serii)
        {
            foreach (var set in serii)
                foreach (var expression in set)
                    this.baseList.Add(expression);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var item in this)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" | ");
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }

        public string GetBodyString()
        {
            return ToString();
        }
    }
}
