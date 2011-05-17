using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRule :
        ReadOnlyCollection<IProductionRuleItem>,
        IProductionRule
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
        /// Data member for <see cref="FileName"/>.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Creates a new <see cref="ProductionRule"/> with the <paramref name="fileName"/>,
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="ProductionRule"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="ProductionRule"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="ProductionRule"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="ProductionRule"/> 
        /// was declared at.</param>
        public ProductionRule(IList<IProductionRuleItem> items, string fileName, int column, int line, long position)
            : base(items)
        {
            this.column = column;
            this.line = line;
            this.fileName = fileName;
            this.position = position;
        }

        #region IScannableEntryItem Members

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="ProductionRule"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="ProductionRule"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="ProductionRule"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Returns the file the <see cref="ProductionRule"/> was declared in.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        #endregion

        #region IAmbiguousGDEntity Members

        public void Disambiguify(IGDFile context, IEntry root)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (IProductionRuleItem ipri in this.baseList)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");
                sb.Append(ipri.ToString());
            }
            return sb.ToString();
        }

        internal ICollection<IProductionRuleItem> BaseCollection
        {
            get
            {
                return base.baseList;
            }
        }

        #region IProductionRule Members


        public IProductionRule Clone()
        {
            List<IProductionRuleItem> result = new List<IProductionRuleItem>();
            foreach (IProductionRuleItem ipri in this)
                result.Add(ipri.Clone());
            return new ProductionRule(result, this.fileName, this.Column, this.Line, this.Position);
        }

        #endregion

        internal void SetItemAt(int index, IProductionRuleItem value)
        {
            IProductionRuleItem[] j = new IProductionRuleItem[this.Count];
            this.baseList.CopyTo(j,0);
            j[index] = value;
            this.baseList.Clear();
            foreach (IProductionRuleItem item in j)
                this.baseList.Add(item);
        }

        
    }
}
