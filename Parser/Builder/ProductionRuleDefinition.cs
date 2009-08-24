using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Parser.Builder
{
    public class ProductionRuleDefinition :
        IProductionRuleDefinition
    {
        private IInterfaceType interfaceForm;
        private IClassType classForm;

        internal ProductionRuleDefinition()
        {
        }

        #region IProductionRuleDefinition Members

        /// <summary>
        /// Returns the interface of the <see cref="ProductionRuleDefinition"/>.
        /// </summary>
        public IInterfaceType InterfaceForm
        {
            get { return this.interfaceForm; }
            internal set { this.interfaceForm = value; }
        }

        /// <summary>
        /// Returns hte <see cref="IClassType"/> of the <see cref="ProductionRuleDefinition"/>
        /// which stores the data for the specific rule definition.
        /// </summary>
        public IClassType ClassForm
        {
            get { return this.classForm; }
            internal set { this.classForm = value; }
        }

        #endregion


    }
}
