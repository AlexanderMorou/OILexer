using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal.Inlining;
using Oilexer.Types.Members;

namespace Oilexer.Parser.Builder
{
    partial class TokenEnumerationData
    {
        public class ItemData
        {
            /// <summary>
            /// Returns the <see cref="IInlinedTokenItem"/> which represents
            /// the item referred to in the <see cref="ItemData"/>.
            /// </summary>
            public IInlinedTokenItem Item { get; private set; }

            /// <summary>
            /// Returns the <see cref="IFieldMember"/> within the current
            /// subset's enumeration.
            /// </summary>
            public IFieldMember SubsetFlagField { get; private set; }

            public IFieldMember FullSetField { get; private set; }

            public IFieldMember TransitionBackingField { get; private set; }

            public IPropertyMember TransitionProperty { get; private set; }

            public int SubsetOffset { get; private set; }

            public int FullSetOffset { get; private set; }
        }
    }
}
