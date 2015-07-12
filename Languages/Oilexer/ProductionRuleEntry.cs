using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
/*---------------------------------------------------------------------\
| Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Provides a base implementation of <see cref="IOilexerGrammarProductionRuleEntry"/> which provides 
    /// a means for working with an <see cref="IOilexerGrammarEntry"/> production rule.
    /// Used to express a part of syntax for a <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public class OilexerGrammarProductionRuleEntry :
        ControlledCollection<IProductionRule>,
        IOilexerGrammarProductionRuleEntry
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
        /// Data member for <see cref="IsRuleCollapsePoint"/>.
        /// </summary>
        private bool elementsAreChildren;
        //#if DEBUG
        internal string debugString;
        //#endif
        internal IProductionRuleCaptureStructure captureStructure;
        private string preexpansionText;

        public OilexerGrammarProductionRuleEntry(string name, EntryScanMode scanMode, string fileName, int column, int line, long position)
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
            this.baseList.Add(rule);
        }
        internal void Clear()
        {
            this.baseList.Clear();
        }

        //#region IOilexerGrammarScannableEntry Members

        public EntryScanMode ScanMode
        {
            get { return this.scanMode; }
        }

        //#endregion

        //#region IOilexerGrammarNamedEntry Members

        /// <summary>
        /// Returns the name of the <see cref="OilexerGrammarNamedEntry"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        //#endregion

        //#region IOilexerGrammarEntry Members

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="OilexerGrammarEntry"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="OilexerGrammarEntry"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="OilexerGrammarEntry"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Returns the file the <see cref="OilexerGrammarEntry"/> was declared in.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        //#endregion

        internal ICollection<IProductionRule> BaseCollection
        {
            get
            {
                return this.baseList;
            }
        }
        public override string ToString()
        {
#if DEBUG
            if (this.debugString == null)
                this.debugString = string.Format("{0}\xA0::={2}\r\n{1};", this.Name, GetBodyString(), elementsAreChildren ? ">" : string.Empty);
            return this.debugString;
#else
            return string.Format("{0} ::={2} {1};", this.Name, GetBodyString(), elementsAreChildren ? ">" : string.Empty);
#endif

        }

        public string GetBodyString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t");
            bool first = true;
            for (int ruleIndex = 0; ruleIndex < this.Count; ruleIndex++)
            //foreach (IProductionRule ite in this.baseList)
            {
                IProductionRule ite = this.baseList[ruleIndex];
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
            var bodyString = sb.ToString();
            return bodyString;
        }

        //#region IOilexerGrammarProductionRuleEntry Members

        /// <summary>
        /// Returns/sets whether the elements of 
        /// the <see cref="OilexerGrammarProductionRuleEntry"/>
        /// inherit the name of the 
        /// <see cref="OilexerGrammarProductionRuleEntry"/>.
        /// </summary>
        public bool IsRuleCollapsePoint
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

        //#endregion

        public bool IsExtract { get; internal set; }

        public IOilexerGrammarProductionRuleEntry ExtractSource { get; internal set; }

        public bool MaxReduce
        {
            get;
            set;
        }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this; }
        }

        internal IProductionRuleCaptureStructure CaptureStructure
        {
            get
            {
                return this.captureStructure;
            }
            set
            {
                this.captureStructure = value;
            }
        }


        public string PreexpansionText
        {
            get { return this.preexpansionText; }
        }

        public void CreatePreexpansionText()
        {
            this.preexpansionText = this.ToString();
        }


        public bool RepresentsAmbiguousContext { get; set; }

    }
}
