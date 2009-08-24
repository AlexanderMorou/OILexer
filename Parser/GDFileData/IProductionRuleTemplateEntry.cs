using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    public struct TemplateArgumentInformation
    {
        public readonly int FixedArguments;
        public readonly int DynamicArguments;
        public readonly int InvalidArguments;
        internal TemplateArgumentInformation(int fixedArguments, int dynamicArguments, int invalidArguments)
        {
            this.FixedArguments = fixedArguments;
            this.DynamicArguments = dynamicArguments;
            this.InvalidArguments = invalidArguments;
        }
    }
    /// <summary>
    /// Defines properties and methods for working with a template for production rules.
    /// Used to express a part of syntax for a <see cref="IGDFile"/>.
    /// </summary>
    public interface IProductionRuleTemplateEntry :
        IProductionRuleEntry
    {
        /// <summary>
        /// The names of the parts associated with the <see cref="IProductionRuleTemplateEntry"/>.
        /// </summary>
        IProductionRuleTemplateParts Parts { get; }
        TemplateArgumentInformation GetArgumentInformation();
    }
}
