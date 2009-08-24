using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{

    /// <summary>
    /// Provides a base implementation of <see cref="ILiteralCharReferenceProductionRuleItem"/>
    /// which is a reference to a literal character from a <see cref="ITokenEntry"/>.
    /// </summary>
    public class LiteralCharReferenceProductionRuleItem :
        LiteralReferenceProductionRuleItem<char, ILiteralCharTokenItem>,
        ILiteralCharReferenceProductionRuleItem
    {

        /// <summary>
        /// Creates a new <see cref="LiteralCharReferenceProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="literal">The <see cref="ILiteralCharTokenItem"/> which the <see cref="LiteralCharReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="source">The <see cref="ITokenEntry"/> which the contains the <paramref name="literal"/> the <see cref="LiteralCharReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was declared.</param>
        public LiteralCharReferenceProductionRuleItem(ILiteralCharTokenItem literal, ITokenEntry source, int column, int line, long position)
            : base(literal, source, column, line, position)
        {
        }
        public LiteralCharReferenceProductionRuleItem(ILiteralCharTokenItem literal, ITokenEntry source, int column, int line, long position, bool wasFlag, bool wasCounter)
            : base(literal, source, column, line, position, wasFlag, wasCounter)
        {
        }

        /// <summary>
        /// Type-inspecific 'onclone' to allowfor hiding of <see cref="Clone()"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralCharReferenceProductionRuleItem"/> instance which is a
        /// copy of the current <see cref="LiteralCharReferenceProductionRuleItem"/>.</returns>
        protected override sealed object OnClone()
        {
            LiteralCharReferenceProductionRuleItem result = new LiteralCharReferenceProductionRuleItem(this.Literal, this.Source, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

        #region ILiteralCharReferenceProductionRuleItem Members

        public new ILiteralCharReferenceProductionRuleItem Clone()
        {
            return ((ILiteralCharReferenceProductionRuleItem)(base.Clone()));
        }

        #endregion

    }
}
