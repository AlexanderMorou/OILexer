using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal;
namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{

    /// <summary>
    /// Provides a base implementation of <see cref="ILiteralStringReferenceProductionRuleItem"/>
    /// which is a reference to a string literal from a <see cref="ITokenEntry"/>.
    /// </summary>
    public class LiteralStringReferenceProductionRuleItem :
        LiteralReferenceProductionRuleItem<string, ILiteralStringTokenItem>,
        ILiteralStringReferenceProductionRuleItem
    {

        /// <summary>
        /// Creates a new <see cref="LiteralStringReferenceProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="literal">The <see cref="ILiteralStringTokenItem"/> which the <see cref="LiteralStringReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="source">The <see cref="ITokenEntry"/> which the contains the <paramref name="literal"/> the <see cref="LiteralStringReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralStringReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralStringReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralStringReferenceProductionRuleItem"/> was declared.</param>
        public LiteralStringReferenceProductionRuleItem(ILiteralStringTokenItem literal, ITokenEntry source, int column, int line, long position)
            : base(literal, source, column, line, position)
        {
        }
        public LiteralStringReferenceProductionRuleItem(ILiteralStringTokenItem literal, ITokenEntry source, int column, int line, long position, bool wasFlag, bool wasCounter)
            : base(literal, source, column, line, position, wasFlag, wasCounter)
        {
        }

        /// <summary>
        /// Type-inspecific 'onclone' to allowfor hiding of <see cref="Clone()"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralStringReferenceProductionRuleItem"/> instance which is a
        /// copy of the current <see cref="LiteralStringReferenceProductionRuleItem"/>.</returns>
        protected override sealed object OnClone()
        {
            LiteralStringReferenceProductionRuleItem result = new LiteralStringReferenceProductionRuleItem(this.Literal, this.Source, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

        #region ILiteralStringReferenceProductionRuleItem Members

        public new ILiteralStringReferenceProductionRuleItem Clone()
        {
            return ((ILiteralStringReferenceProductionRuleItem)(base.Clone()));
        }

        #endregion

        public override string ToString()
        {
            if (this.Name != null)
                return string.Format("{0}{3}{4}:{1};{2}{5}", this.Literal.Value.Encode(), this.Name, this.ToStringFurtherOptions(), this.Counter ? "#" : string.Empty, this.IsFlag ? "!" : string.Empty, this.RepeatOptions);
            else
                return string.Format("{0}{2}{3}{1}{4}", this.Literal.Value.Encode(), this.ToStringFurtherOptions(), this.Counter ? "#" : string.Empty, this.IsFlag ? "!" : string.Empty, this.RepeatOptions);
        }

    }
}
