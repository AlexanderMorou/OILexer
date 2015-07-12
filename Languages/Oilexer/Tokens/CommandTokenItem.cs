using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public abstract class CommandTokenItem :
        TokenItem,
        ICommandTokenItem
    {
        private ITokenExpressionSeries[] commandArguments;

        public CommandTokenItem(ITokenExpressionSeries[] commandArguments, int column, int line, long position)
            : base(column, line, position)
        {
            this.commandArguments = commandArguments;
        }


        #region ICommandTokenItem Members

        /// <summary>
        /// Returns the <see cref="CommandType"/> associated to the command.
        /// </summary>
        public abstract CommandType Type { get; }

        public IEnumerable<ITokenExpressionSeries> Arguments
        {
            get
            {
                foreach (var series in commandArguments)
                    yield return series;
            }
        }

        #endregion
    }
}
