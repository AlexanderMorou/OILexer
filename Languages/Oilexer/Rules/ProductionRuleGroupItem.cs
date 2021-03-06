﻿using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleGroupItem :
        ControlledCollection<IProductionRule>,
        IProductionRuleGroupItem
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
        private ScannableEntryItemRepeatInfo repeatOptions;

        public ProductionRuleGroupItem(ICollection<IProductionRuleSeries> serii, int column, int line, long position)
        {
            this.line = line;
            this.column = column;
            this.position = position;
            foreach (IProductionRuleSeries iprs in serii)
                foreach (IProductionRule ipr in iprs)
                    this.baseList.Add(ipr);
        }

        public ProductionRuleGroupItem(IProductionRule[] items, int column, int line, long position)
        {
            this.line = line;
            this.column = column;
            this.position = position;
            foreach (IProductionRule ipr in items)
                baseList.Add(ipr);
        }

        public ProductionRuleGroupItem(Dictionary<string, string> constraints, IProductionRule[] items, int column, int line, long position)
            : this(items, column, line, position)
        {
            this.ConditionalConstraints = new ControlledDictionary<string, string>(constraints);
        }

        public ProductionRuleGroupItem(Dictionary<string, string> constraints, ICollection<IProductionRuleSeries> serii, int column, int line, long position)
            : this(serii, column, line, position)
        {
            this.ConditionalConstraints = new ControlledDictionary<string, string>(constraints);
        }

        public IControlledDictionary<string, string> ConditionalConstraints { get; private set; }

        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleGroupItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRuleGroupItem"/> with the data
        /// members of the current <see cref="IProductionRuleGroupItem"/>.</returns>
        public IProductionRuleGroupItem Clone()
        {
            IProductionRule[] ipr = new IProductionRule[this.Count];
            base.CopyTo(ipr, 0);
            ProductionRuleGroupItem prgi = new ProductionRuleGroupItem(ipr, this.Column, this.Line, this.Position);
            if (this.ConditionalConstraints != null)
                prgi.ConditionalConstraints = this.ConditionalConstraints;
            prgi.repeatOptions = this.repeatOptions;
            prgi.name = this.name;
            return prgi;
        }

        //#region IScannableEntryItem Members

        /// <summary>
        /// Returns the name of the <see cref="ScannableEntryItem"/>, if it was defined.
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
        /// Returns the repeat options of the <see cref="ScannableEntryItem"/>
        /// </summary>
        public ScannableEntryItemRepeatInfo RepeatOptions
        {
            get { return this.repeatOptions; }
            set
            {
                this.repeatOptions = value;
            }
        }

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="ScannableEntryItem"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        IScannableEntryItem IScannableEntryItem.Clone()
        {
            return this.Clone();
        }

        //#endregion

        //#region IProductionRuleItem Members

        IProductionRuleItem IProductionRuleItem.Clone()
        {
            return this.Clone();
        }

        //#endregion
        public override string ToString()
        {
            return BuildString();
        }

        private string BuildString(bool appendExtra = true)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            if (appendExtra)
            {
                if (this.Count > 1 || this.Count > 0 && this[0].Count > 1)
                    sb.AppendLine();
                sb.Append("(");
                if (this.Count > 1 || this.Count > 0 && this[0].Count > 1)
                {
                    sb.AppendLine();
                    sb.Append("\t");
                }
            }
            foreach (IProductionRule ite in this.baseList)
            {
                if (first)
                    first = false;
                else
                {
                    sb.AppendLine(" |");
                    sb.Append("\t");
                }
                var current = ite.ToString();
                if (current.Length > Environment.NewLine.Length)
                {
                    if (current.Substring(0, Environment.NewLine.Length) == Environment.NewLine)
                        current = current.Substring(Environment.NewLine.Length);
                    if (current.Substring(current.Length - Environment.NewLine.Length) == Environment.NewLine)
                        current = current.Substring(0, current.Length - Environment.NewLine.Length);
                }
                sb.Append(current.Replace(Environment.NewLine, Environment.NewLine + "\t"));
            }
            if (appendExtra)
            {
                if (this.Count > 1 || this.Count > 0 && this[0].Count > 1)
                    sb.AppendLine();
                sb.Append(")");
                if (this.name != null && this.name != string.Empty)
                    sb.Append(string.Format(":{0};", this.Name));
                sb.Append(repeatOptions.ToString());
            }
            return sb.ToString();
        }


        public string GetBodyString()
        {
            return BuildString(false);
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
