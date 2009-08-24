using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    partial class RuleDataSet
    {
        internal struct NamedSet
            : IEnumerable<IRuleDataNamedItem>
        {
            private RuleDataSet source;
            public string Name { get; private set; }
            public NamedSet(RuleDataSet source, string name)
                : this()
            {
                this.source = source;
                this.Name = name;
            }

            #region IEnumerable<IRuleDataNamedItem> Members

            public IEnumerator<IRuleDataNamedItem> GetEnumerator()
            {
                if (this.source == null)
                    yield break;
                foreach (var element in source)
                {
                    if (element is IRuleDataNamedItem)
                    {
                        var current = (IRuleDataNamedItem)element;
                        if (current.Name == this.Name)
                            yield return current;
                    }
                }
                yield break;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
    }
}
