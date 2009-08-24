using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Provides a base implementation of <see cref="IProductionRuleEntry"/> which provides 
    /// a means for working with an <see cref="IEntry"/> production rule.
    /// Used to express a part of syntax for a <see cref="IGDFile"/>.
    /// </summary>
    public class ProductionRuleEntry :
        ReadOnlyCollection<IProductionRule>,
        IProductionRuleEntry
    {
        private EntryScanMode scanMode;
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
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="ElementsAreChildren"/>.
        /// </summary>
        private bool elementsAreChildren;
        public ProductionRuleEntry(string name, EntryScanMode scanMode, string fileName, int column, int line, long position)
        {
            this.scanMode = scanMode;
            this.column = column;
            this.line = line;
            this.fileName = fileName;
            this.position = position;
            this.name = name;
        }

        internal void Add(IProductionRule rule)
        {
            this.baseCollection.Add(rule);
        }
        internal void Clear()
        {
            this.baseCollection.Clear();
        }

        #region IScannableEntry Members

        public EntryScanMode ScanMode
        {
            get { return this.scanMode; }
        }

        #endregion

        #region INamedEntry Members

        /// <summary>
        /// Returns the name of the <see cref="NamedEntry"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion

        #region IEntry Members

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="Entry"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="Entry"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="Entry"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Returns the file the <see cref="Entry"/> was declared in.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        #endregion

        internal ICollection<IProductionRule> BaseCollection
        {
            get
            {
                return this.baseCollection;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (IProductionRule ite in this.baseCollection)
            {
                if (first)
                    first = false;
                else
                {
                    sb.AppendLine(" | ");
                    sb.Append("\t");
                }
                sb.Append(ite.ToString().Replace("\r\n\t", "\r\n\t\t"));
            }
            return string.Format("{0} ::={2} {1};", this.Name, sb.ToString(), elementsAreChildren ? ">" : string.Empty);

        }

        #region IProductionRuleEntry Members

        /// <summary>
        /// Returns/sets whether the elements of 
        /// the <see cref="ProductionRuleEntry"/>
        /// inherit the name of the 
        /// <see cref="ProductionRuleEntry"/>.
        /// </summary>
        public bool ElementsAreChildren
        {
            get
            {
                return this.elementsAreChildren;
            }
            set
            {
                this.elementsAreChildren = value;
            }
        }

        #endregion

        public bool IsExtract { get; internal set; }

        public IProductionRuleEntry ExtractSource { get; internal set; }
    }
}
