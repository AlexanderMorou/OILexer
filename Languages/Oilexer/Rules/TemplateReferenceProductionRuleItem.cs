﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TemplateReferenceProductionRuleItem :
        ControlledCollection<IProductionRuleSeries>,
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
        private ScannableEntryItemRepeatInfo repeatOptions;
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IOilexerGrammarProductionRuleTemplateEntry reference;

        /// <summary>
        /// Creates a new <see cref="TemplateReferenceProductionRuleItem"/> with the <paramref name="reference"/>,
        /// <paramref name="parts"/>, <paramref name="column"/>, <paramref name="line"/>, and 
        /// <paramref name="position"/> provided.
        /// </summary>
        /// <param name="reference">The <see cref="IOilexerGrammarProductionRuleTemplateEntry"/> which
        /// is referenced by the <see cref="TemplateReferenceProductionRuleItem"/>.</param>
        /// <param name="parts">The <see cref="IProductionRuleSeries"/> collection which denotes the
        /// parameters passed to the template expansion request.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TemplateReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TemplateReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TemplateReferenceProductionRuleItem"/> was declared.</param>
        public TemplateReferenceProductionRuleItem(IOilexerGrammarProductionRuleTemplateEntry reference, IList<IProductionRuleSeries> parts, int column, int line, long position)
            : base(parts)
        {
            this.column = column;
            this.line = line;
            this.position = position;
            this.reference = reference;
        }

        public TemplateReferenceProductionRuleItem(Dictionary<string, string> constraints, IOilexerGrammarProductionRuleTemplateEntry reference, ICollection<IProductionRuleSeries> parts, int column, int line, long position)
        {
            this.ConditionalConstraints = new ControlledDictionary<string, string>(constraints);
            
            foreach (IProductionRuleSeries iprs in parts)
                baseList.Add(iprs);
            this.column = column;
            this.line = line;
            this.position = position;
            this.reference = reference;
            
        }
        public IControlledDictionary<string, string> ConditionalConstraints { get; private set; }

        //#region ITemplateReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleTemplateEntry"/> which the 
        /// <see cref="ITemplateReferenceProductionRuleItem"/> references.
        /// </summary>
        public IOilexerGrammarProductionRuleTemplateEntry Reference
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
            IList<IProductionRuleSeries> r = new List<IProductionRuleSeries>();
            foreach (IProductionRuleSeries iprs in this)
                r.Add(iprs);
            TemplateReferenceProductionRuleItem trpri = new TemplateReferenceProductionRuleItem(this.Reference, r, this.Column, this.Line, this.Position);
            if (this.ConditionalConstraints != null)
                trpri.ConditionalConstraints = this.ConditionalConstraints;
            trpri.RepeatOptions = this.repeatOptions;
            trpri.Name = Name;
            return trpri;
        }

        //#endregion

        //#region IProductionRuleItem Members

        IProductionRuleItem IProductionRuleItem.Clone()
        {
            return this.Clone();
        }

        //#endregion
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

        //#region IScannableEntryItem Members

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
        public ScannableEntryItemRepeatInfo RepeatOptions
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

        //#endregion
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            if (this.Count > 1)
                sb.AppendLine();
            sb.Append(this.Reference.Name);
            if (this.Count > 1 || this.Count > 0 && this[0].Count > 1 || this.Count > 0 && this[0].Count > 0 && this[0][0].Count > 1)
            {
                sb.AppendLine("<");
                sb.Append("\t");
            }
            else
                sb.Append("<");
            foreach (var item in this)
            {
                if (first)
                    first = false;
                else
                {
                    sb.AppendLine(",");
                    sb.Append("\t");
                }
                var current = item.ToString();
                if (current.Length > Environment.NewLine.Length)
                {
                    if (current.Substring(0, Environment.NewLine.Length) == Environment.NewLine)
                        current = current.Substring(Environment.NewLine.Length);
                    if (current.Substring(current.Length - Environment.NewLine.Length) == Environment.NewLine)
                        current = current.Substring(0, current.Length - Environment.NewLine.Length);
                }
                sb.Append(current.Replace(Environment.NewLine, Environment.NewLine + "\t"));
            }
            if (this.Count > 1 || this.Count > 0 && this[0].Count > 1 || this.Count > 0 && this[0].Count > 0 && this[0][0].Count > 1)
                sb.AppendLine();
            sb.Append(">");
            if (this.name != null && this.name != string.Empty)
            {
                sb.Append(":");
                sb.Append(this.Name);
                sb.Append(";");
            }
            sb.Append(repeatOptions.ToString());
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
            if (target.RepeatOptions == ScannableEntryItemRepeatInfo.None
             && target.RepeatOptions != this.RepeatOptions)
                target.RepeatOptions = this.repeatOptions;
            if (!string.IsNullOrEmpty(this.name))
                if (target.Name != this.Name)
                    target.Name = name;
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
