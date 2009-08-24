using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public class TemplateReferenceProductionRuleItem :
        ReadOnlyCollection<IProductionRuleSeries>,
        ITemplateReferenceProductionRuleItem
    {
        /// <summary>
        /// Data member for <see cref="Column"/>.
        /// </summary>
        private int column;
        /// <summary>
        /// Data member for <see cref="Line"/>.
        /// </summary>
        private int line;
        /// <summary>
        /// Data member for <see cref="Position"/>.
        /// </summary>
        private long position;

        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="RepeatOptions"/>.
        /// </summary>
        private ScannableEntryItemRepeatOptions repeatOptions;
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IProductionRuleTemplateEntry reference;

        /// <summary>
        /// Creates a new <see cref="TemplateReferenceProductionRuleItem"/> with the <paramref name="reference"/>,
        /// <paramref name="parts"/>, <paramref name="column"/>, <paramref name="line"/>, and 
        /// <paramref name="position"/> provided.
        /// </summary>
        /// <param name="reference">The <see cref="IProductionRuleTemplateEntry"/> which
        /// is referenced by the <see cref="TemplateReferenceProductionRuleItem"/>.</param>
        /// <param name="parts">The <see cref="IProductionRuleSeries"/> collection which denotes the
        /// parameters passed to the template expansion request.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TemplateReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TemplateReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TemplateReferenceProductionRuleItem"/> was declared.</param>
        public TemplateReferenceProductionRuleItem(IProductionRuleTemplateEntry reference, ICollection<IProductionRuleSeries> parts, int column, int line, long position)
        {
            foreach (IProductionRuleSeries iprs in parts)
                baseCollection.Add(iprs);
            this.column = column;
            this.line = line;
            this.position = position;
            this.reference = reference;
        }


        #region ITemplateReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the <see cref="IProductionRuleTemplateEntry"/> which the 
        /// <see cref="ITemplateReferenceProductionRuleItem"/> references.
        /// </summary>
        public IProductionRuleTemplateEntry Reference
        {
            get
            {
                return this.reference;
            }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="TemplateReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="TemplateReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="TemplateReferenceProductionRuleItem"/>.</returns>
        public ITemplateReferenceProductionRuleItem Clone()
        {
            ICollection<IProductionRuleSeries> r = new System.Collections.ObjectModel.Collection<IProductionRuleSeries>();
            foreach (IProductionRuleSeries iprs in this)
                r.Add(iprs);
            TemplateReferenceProductionRuleItem trpri = new TemplateReferenceProductionRuleItem(this.Reference, r, this.Column, this.Line, this.Position);
            trpri.RepeatOptions = this.repeatOptions;
            trpri.Name = Name;
            return trpri;
        }

        #endregion

        #region IProductionRuleItem Members

        IProductionRuleItem IProductionRuleItem.Clone()
        {
            return this.Clone();
        }

        #endregion
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="TemplateReferenceProductionRuleItem"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="TemplateReferenceProductionRuleItem"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="TemplateReferenceProductionRuleItem"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        #region IScannableEntryItem Members

        /// <summary>
        /// Returns the name of the <see cref="TemplateReferenceProductionRuleItem"/>, if it was defined.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Returns the repeat options of the <see cref="TemplateReferenceProductionRuleItem"/>
        /// </summary>
        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get { return this.repeatOptions; }
            set
            {
                this.repeatOptions = value;
            }
        }

        IScannableEntryItem IScannableEntryItem.Clone()
        {
            return this.Clone();
        }

        #endregion
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append(this.Reference.Name);
            sb.Append("<");
            foreach (var item in this)
            {
                if (first)
                    first = false;
                else
                    sb.Append(",");
                sb.Append(item.ToString());
            }

            sb.Append(">");
            if (this.name != null && this.name != string.Empty)
            {
                sb.Append(":");
                sb.Append(this.Name);
                sb.Append(";");
            }
            switch (repeatOptions)
            {
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    sb.Append("?");
                    break;
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    sb.Append("*");
                    break;
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    sb.Append("+");
                    break;
            }
            return sb.ToString();
        }
        /// <summary>
        /// Copies the data-members of the current <see cref="ScannableEntryItem"/> to the <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The <see cref="IScannableEntryItem"/> which needs
        /// the data copied.</param>
        /// <remarks>Supplement to <see cref="OnClone()"/>.</remarks>
        internal void CloneData(IScannableEntryItem target)
        {
            if (target.RepeatOptions == ScannableEntryItemRepeatOptions.None
             && target.RepeatOptions != this.RepeatOptions)
                target.RepeatOptions = this.repeatOptions;
            if (!string.IsNullOrEmpty(this.name))
                if (target.Name != this.Name)
                    target.Name = name;
        }

    }
}
