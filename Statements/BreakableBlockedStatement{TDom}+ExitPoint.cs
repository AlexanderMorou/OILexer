using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Statements
{
    partial class BreakableBlockedStatement<TDom>
    {
        [Serializable]
        public class ExitPoint :
            LabelStatement,
            IBreakTargetExitPoint
        {
            private bool skip;
            public ExitPoint(IStatementBlock sourceBlock)
                : base(sourceBlock, GetName(sourceBlock))
            {
                this.skip = true;
            }

            public override bool Skip
            {
                get
                {
                    return this.skip;
                }
                set
                {
                    this.skip = value;
                }
            }

            private static string GetName(IStatementBlock sourceBlock)
            {
                if (sourceBlock.Parent.DefinedLabelNames != null)
                {
                    string baseName = "__exit";
                    string itemName = baseName;
                    int i = 0;
                    while (sourceBlock.Parent.DefinedLabelNames.Contains(itemName))
                    {
                        i++;
                        itemName = string.Format("{0}_{1}", baseName, i);
                    }
                    sourceBlock.Parent.DefinedLabelNames.Add(itemName);
                    return itemName;
                }
                else
                    return "__exit";
            }
        }
    }
}
