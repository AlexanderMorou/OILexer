using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedTokenEntry :
        FlattenedTokenExpressionSeries
    {
        public readonly ITokenEntry Source;
        /// <summary>
        /// Creates a new <see cref="FlattenedTokenEntry"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/> 
        /// from which the current <see cref="FlattenedTokenEntry"/>
        /// is based upon.</param>
        public FlattenedTokenEntry(ITokenEntry source)
            : base(source.Branches)
        {
            this.Source = source;
        }

        public new void Initialize()
        {
            base.Initialize();
            if (this.startState == null)
                base.GetState();
        }

    }
}
