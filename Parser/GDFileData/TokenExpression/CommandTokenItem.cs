using System;
using System.Collections.Generic;
using System.Text;
using Oilexer._Internal;

namespace Oilexer.Parser.GDFileData.TokenExpression
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
